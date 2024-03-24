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
  private Panel DeathScreen;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fuelDisplay = GetNode<Label>("InGame/FuelDisplay");
    healthDisplay = GetNode<Label>("InGame/HealthDisplay");
    PauseMenu = GetNode<Panel>("PauseMenu");
    DeathScreen = GetNode<Panel>("DeathScreen");
    GetNode<gameController>("../GameController").GamePaused += _OnGamePaused;
    PauseMenu.Visible = false;
    DeathScreen.Visible = false;
    GetNode<gameController>("../GameController").PlayerDead += _OnPlayerDead;
    DashBar = GetNode<ProgressBar>("InGame/DashBar");
    DashBar.Value = 100;

    healthDisplay.Text = "Health: " + player.CurrentHealth.ToString();

    player.DashStart += _OnDashStart;
    dashCooldown = player.GetNode<Timer>("DashCooldown");
    PauseMenu.GetNode<Panel>("HelpPanel").Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fuelDisplay.Text = "Fuel: " + Mathf.Floor(player.JetPackStamina).ToString();
    healthDisplay.Text = "Health: " + player.CurrentHealth.ToString();
	}

  public override void _PhysicsProcess(double delta)
  {
    if(!dashCooldown.IsStopped()) {
      DashBar.Value = (1 - dashCooldown.TimeLeft / dashCooldown.WaitTime) * 100;
    }
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

  public void _OnPlayerDead(bool playerDead){
    DeathScreen.GetNode<Label>("DeathContainer/DeathLabel").Text = "PLUGIN YOUR CONTROLLER";
    DeathScreen.Visible = true;
    DeathScreen.GetNode<Button>("DeathContainer/Restart").GrabFocus();
  }

  public void _OnPlayerFinished(){
    DeathScreen.GetNode<Label>("DeathContainer/DeathLabel").Text = "YOU WIN";
    DeathScreen.Visible = true;
    DeathScreen.GetNode<Button>("DeathContainer/Restart").GrabFocus();
  }

  public void _OnRestartButtonDown(){
    GetTree().Paused = false;
    GetTree().ReloadCurrentScene();
  }
}
