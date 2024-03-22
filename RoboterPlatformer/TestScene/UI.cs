using Godot;
using System;

public partial class UI : Control
{
	private Player player;
	private TextEdit fuelDisplay;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("../Player");
		fuelDisplay = GetNode<TextEdit>("FuelDisplay");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fuelDisplay.Text = "Fuel: " + Mathf.Floor(player.JetPackStamina).ToString();
	}
}