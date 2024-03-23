using Godot;

public partial class UI : Control
{
  [Export]
	private Player player;
	private Label fuelDisplay;
  private Label healthDisplay;
  private Panel PauseMenu;
  private ProgressBar DashBar;
  private Timer dashCooldown;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fuelDisplay = GetNode<Label>("InGame/FuelDisplay");
    healthDisplay = GetNode<Label>("InGame/HealthDisplay");
    PauseMenu = GetNode<Panel>("PauseMenu");
    PauseMenu.Visible = false;

    DashBar = GetNode<ProgressBar>("InGame/DashBar");
    DashBar.Value = 100;

    healthDisplay.Text = "Health: " + player.CurrentHealth.ToString();

    player.HealthChanged += OnHealthChanged;
    player.DashStart += _OnDashStart;
    dashCooldown = player.GetNode<Timer>("DashCooldown");
    PauseMenu.GetNode<Panel>("HelpPanel").Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fuelDisplay.Text = "Fuel: " + Mathf.Floor(player.JetPackStamina).ToString();
	}

  public override void _PhysicsProcess(double delta)
  {
    if(!dashCooldown.IsStopped()) {
      DashBar.Value = (1 - dashCooldown.TimeLeft / dashCooldown.WaitTime) * 100;
    }
  }

  public void OnHealthChanged(float oldValue, float newValue) {
    healthDisplay.Text = "Health: " + newValue.ToString();
  }

  public void _OnGamePaused(bool isPaused) {
    PauseMenu.Visible = isPaused;
    if(isPaused)  {
      PauseMenu.GetNode<Button>("Container/Continue").GrabFocus();
      PauseMenu.GetNode<Panel>("HelpPanel").Visible = false;
    }
  }

  public void _OnContinueButtonDown() {
    Input.ActionPress("ui_cancel");
    Input.ActionRelease("ui_cancel");
  }

  public void _OnExitButtonDown() {
    GetTree().Quit();
  }

  public void _OnDashStart() {
    DashBar.Value = 0;
  }

  public void _OnHelpButtonDown() {
    PauseMenu.GetNode<Panel>("HelpPanel").Visible = true;
  }

  public void _OnHelpBackButtonDown() {
    PauseMenu.GetNode<Panel>("HelpPanel").Visible = false;
  }
}
