using Godot;

public partial class Checkpoint : Area3D
{
  [Export]
  private Material checkedMaterial;
  [Signal]
  public delegate void PlayerRegisteredEventHandler(Checkpoint checkpoint);
  private bool isActive;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
    BodyEntered += _OnBodyEntered;
    PlayerRegistered += GetNode<gameController>("../GameController")._OnPlayerRegister;
	}

  public void _OnBodyEntered(Node3D body) {
    if(!isActive && body is Player) {
      isActive = true;
      GetChild<MeshInstance3D>(0).MaterialOverride = checkedMaterial;
      EmitSignal(SignalName.PlayerRegistered, this);
    }
  }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

  public void SetActive(bool active) {
    isActive = active;
    if(active)
      GetChild<MeshInstance3D>(0).MaterialOverride = checkedMaterial;
    else
      GetChild<MeshInstance3D>(0).MaterialOverride = null;
  }
}
