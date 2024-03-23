using Godot;
using System;

public partial class gameController : Node
{
	[Export]
	Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player.HealthChanged += OnPlayerHealthChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnPlayerHealthChanged(float oldHealth, float Health){
		if(Health <= 0){
			GD.Print("Dead!");
		}
	}
}
