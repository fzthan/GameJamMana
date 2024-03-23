using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class fan : Node3D
{

	[Export]
	public float force = 1.5f;
	[Export]
	public float rotationSpeed = 0.05f;
	[Export]
	public bool active = true;
	private MeshInstance3D rotor;
	private Vector3 direction = new Vector3(0,0,1);
	private Player obj;
	private bool activePush = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rotor = GetNode<MeshInstance3D>("Rotor");
		direction = direction.Rotated(new Vector3(0,1,0), GlobalRotation.Y);
	}

	public void _on_body_entered(Node3D body){
		if(body is Player && active){
			obj = (Player)body;
			activePush = true;
		}
	}

	public void _on_body_exited(Node3D body){
		if(body == obj){
			activePush = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		rotor.RotateZ(rotationSpeed);
		if(activePush){
			 obj.Velocity += direction * force;
			 obj.MoveAndSlide();
		}
	}
}
