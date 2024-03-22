using Godot;
using System;


public partial class electro_spike : Area3D
{
	CharacterBody3D playerNode;
	[Export]
	private float damage = 1;
	[Export]
	public bool active = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void _on_body_entered(Node3D body){
		if(body is Player && active){
			body.Health -= damage;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
