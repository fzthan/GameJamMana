using Godot;
using System;

public partial class RoboterRig : Node3D
{
	[Export]
	private StandardMaterial3D texture;
	[Export]
	private CompressedTexture2D oldTexture;
	[Export]
	private CompressedTexture2D newTexture;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_SwitchTexture(1);
		GetNode<gameController>("../../../GameController").PlayerDied += _SwitchTexture;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _SwitchTexture(int live){
		switch(live){
			case 1:
			texture.AlbedoTexture = oldTexture;
			break;
			case 2:
			texture.AlbedoTexture = newTexture;
			break;
		}
	}

}
