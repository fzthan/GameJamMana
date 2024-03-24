using Godot;

public partial class Checkpoint : Area3D
{
  [Signal]
  public delegate void PlayerRegisteredEventHandler(Checkpoint checkpoint);
  public enum SPAWN_LOCATIONS {
    LEFT = 0b0000001, RIGHT = 0b0000010, FRONT = 0b0000100
  }
  [Export]
  public SPAWN_LOCATIONS SpawnLocation { get; private set; } = SPAWN_LOCATIONS.LEFT;
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
      EmitSignal(SignalName.PlayerRegistered, this);
    }
  }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

  public void SetActive(bool active) {
    isActive = active;
  }
}
