using Godot;
using System;

public partial class Finish : Area3D
{
	UI ui;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ui = GetNode<UI>("../UI");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnBodyEntered(Node3D body){
		if(body is Player){
			GetTree().Paused = true;
			Input.MouseMode = Input.MouseModeEnum.Visible;
			ui._OnPlayerFinished();
		}
	}
}
