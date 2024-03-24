using System;
using Godot;

public partial class UI : Control
{
  [Export]
	private Player player;
	private Label fuelDisplay;
  private Label healthDisplay;
  private Panel PauseMenu;
  private Label timerDisplay;
  private double time = 0;
  private ProgressBar DashBar;
  private Timer dashCooldown;
  private Panel DeathScreen;
  private bool gameStoped = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fuelDisplay = GetNode<Label>("InGame/FuelDisplay");
    healthDisplay = GetNode<Label>("InGame/HealthDisplay");
    timerDisplay = GetNode<Label>("InGame/Timer");
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
    time += delta;
    if(!gameStoped) timerDisplay.Text = "Timer: " + Math.Round(time, 2, MidpointRounding.ToEven).ToString();
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
    gameStoped = true;
    DeathScreen.GetNode<Label>("DeathContainer/DeathLabel").Text = "PLUGIN YOUR CONTROLLER";
    DeathScreen.GetNode<Label>("DeathContainer/Time").Text = "Your Time: " + Math.Round(time, 2, MidpointRounding.ToEven).ToString();
    DeathScreen.Visible = true;
    DeathScreen.GetNode<Button>("DeathContainer/Restart").GrabFocus();
  }

  public void _OnPlayerFinished(){
    gameStoped = true;
    DeathScreen.GetNode<Label>("DeathContainer/DeathLabel").Text = "YOU WIN";
    DeathScreen.GetNode<Label>("DeathContainer/Time").Text = "Your Time: " + Math.Round(time, 2, MidpointRounding.ToEven).ToString();
    DeathScreen.Visible = true;
    DeathScreen.GetNode<Button>("DeathContainer/Restart").GrabFocus();
  }

  public void _OnRestartButtonDown(){
    GetTree().Paused = false;
    gameStoped = false;
    GetTree().ReloadCurrentScene();
  }
}
