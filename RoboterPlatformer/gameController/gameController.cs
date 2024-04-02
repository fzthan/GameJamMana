using Godot;
using System;

public partial class gameController : Node
{
	[Export]
	Player player;
	[Signal]
	public delegate void GamePausedEventHandler(bool isPaused);
	[Signal]
	public delegate void PlayerDeadEventHandler(bool playerDead);
	[Signal]
	public delegate void PlayerDiedEventHandler(int live);
	[Export]
	public Checkpoint activeCheckpoint;
	private int live = 2;
	private bool isPaused = false;
	private bool playerDead = false;
  private AudioStreamPlayer explosionSoundPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player.HealthChanged += OnPlayerHealthChanged;
		activeCheckpoint.SetActive(true);
    explosionSoundPlayer = GetNode<AudioStreamPlayer>("ExplosionSoundPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			isPaused = !isPaused;
			EmitSignal(SignalName.GamePaused, isPaused);
			GetTree().Paused = isPaused;
			if (isPaused)
				Input.MouseMode = Input.MouseModeEnum.Visible;
			else
				Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		if(Input.IsActionJustPressed("restart")) {
      isPaused = !isPaused;
      GetTree().Paused = false;
      EmitSignal(SignalName.GamePaused, false);
      GetTree().ReloadCurrentScene();
    }

	}

	private async void OnPlayerHealthChanged(float oldHealth, float Health)
	{
		if (Health <= 0 && live == 2)
		{
      explosionSoundPlayer.Play();
			await ToSignal(GetTree().CreateTimer(1), "timeout");
			EmitSignal(SignalName.PlayerDied, live);
			live--;
			switch (activeCheckpoint.SpawnLocation)
			{
				case Checkpoint.SPAWN_LOCATIONS.LEFT:
					player.ResetAndReposition(activeCheckpoint.GetNode<Node3D>("SpawnLocations/Left").GlobalPosition);
					break;
				case Checkpoint.SPAWN_LOCATIONS.RIGHT:
					player.ResetAndReposition(activeCheckpoint.GetNode<Node3D>("SpawnLocations/Right").GlobalPosition);
					break;
				case Checkpoint.SPAWN_LOCATIONS.FRONT:
					player.ResetAndReposition(activeCheckpoint.GetNode<Node3D>("SpawnLocations/Front").GlobalPosition);
					break;
			}
		}
		else if (Health <= 0 && live < 2)
		{
      explosionSoundPlayer.Play();
			await ToSignal(GetTree().CreateTimer(1), "timeout");
			playerDead = true;
			EmitSignal(SignalName.PlayerDead, playerDead);
			EmitSignal(SignalName.PlayerDied, live);
			GetTree().Paused = true;
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	public void _OnPlayerRegister(Checkpoint checkpoint)
	{
		activeCheckpoint.SetActive(false);
		activeCheckpoint = checkpoint;
	}

	public void _AddLive()
	{
		if (live < 2) live++;
	}
}
