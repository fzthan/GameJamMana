using Godot;
using System;


public partial class electro_spike : Area3D
{
	CharacterBody3D playerNode;
	float damage = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerNode = GetNode<Player>("/root/Test/Player");
	}

	public void _on_body_entered(Player body){
		if(body == playerNode){
			body.Health -= damage;
			GD.Print(body.Health);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
