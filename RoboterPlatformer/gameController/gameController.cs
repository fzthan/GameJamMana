using Godot;
using System;

public partial class gameController : Node
{
	[Export]
	Player player;
  [Signal]
  public delegate void GamePausedEventHandler(bool isPaused);

  private bool isPaused = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player.HealthChanged += OnPlayerHealthChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    if(Input.IsActionJustPressed("ui_cancel")) {
      isPaused = !isPaused;
      EmitSignal(SignalName.GamePaused, isPaused);
      GetTree().Paused = isPaused;
      if(isPaused)
        Input.MouseMode = Input.MouseModeEnum.Visible;
      else
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
	}

	private void OnPlayerHealthChanged(float oldHealth, float Health){
		if(Health <= 0){
			GD.Print("Dead!");
		}
	}

  public void _OnPlayerRegister(Checkpoint checkpoint) {
    GD.Print("her4e");
  }
}
