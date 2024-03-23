using Godot;
using System;

public partial class fallingPlatform : RigidBody3D
{
	[Export]
	public const int LowerBound = -10;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void _on_body_entered(Node3D body){
		if(body is Player){
			GetNode<Timer>("Timer").Start();
		}
	}

	public void _on_timer_timeout(){
		this.Freeze = false;
	}

  public override void _PhysicsProcess(double delta)
  {
    base._PhysicsProcess(delta);
	if(GlobalPosition.Y <LowerBound){
		QueueFree();
	}
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
	{
	}
}
