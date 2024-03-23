using Godot;

public partial class UI : Control
{
	private Player player;
	private Label fuelDisplay;
  private Label healthDisplay;
  private Panel PauseMenu;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("../Player");
		fuelDisplay = GetNode<Label>("FuelDisplay");
    healthDisplay = GetNode<Label>("HealthDisplay");
    PauseMenu = GetNode<Panel>("PauseMenu");
    PauseMenu.Visible = false;

    healthDisplay.Text = "Health: " + player.CurrentHealth.ToString();

    player.HealthChanged += OnHealthChanged;


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fuelDisplay.Text = "Fuel: " + Mathf.Floor(player.JetPackStamina).ToString();
	}

  public void OnHealthChanged(float oldValue, float newValue) {
    healthDisplay.Text = "Health: " + newValue.ToString();
  }

  public void _OnGamePaused(bool isPaused) {
    PauseMenu.Visible = isPaused;
  }
}
