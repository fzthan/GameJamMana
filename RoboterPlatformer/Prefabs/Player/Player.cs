using Godot;
using Vector3 = Godot.Vector3;
using Vector2 = Godot.Vector2;
using System.Numerics;
using System;

public partial class Player : CharacterBody3D
{
	private float Health = 10.0f;
	private bool firstLife = false;
	private bool secondLife = false;
	[Export]
	private const float startingHealth = 10.0f;
	public float CurrentHealth { get { return Health; } }
	[Signal]
	public delegate void HealthChangedEventHandler(float oldValue, float newValue);
	[Export]
	public const float Speed = 5.0f;
	[Export]
	public const int LowerBound = -10;
	#region jumping-jetpack
	[Export]
	public float JumpVelocity = 6.5f;
	[Export]
	public float DoubleJumpForce = 5.5f;
	[Export]
	public float JetPackStamina = 20.0f;
	[Export]
	public float JetPackForce = 8.0f;
	private bool HasDoubleJumped = false;
  public bool _hasDoubleJumped {get {return HasDoubleJumped;} }
	#endregion

	#region dashing
	public bool IsDashing = false;
	[Export]
	public float DashSpeed = 50.0f;
	private Timer DashTimer;
	private Timer DashCooldown;
	[Signal]
	public delegate void DashStartEventHandler();
	#endregion
	private Node3D PlayerPivot { get; set; }

	private Vector3 direction = Vector3.Zero;

	#region Camera
	[Export(PropertyHint.Range, "0.1,1.0")] public float camSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] float minCamPitch = -50f;
	[Export(PropertyHint.Range, "0,90,1")] float maxCamPitch = 3;
	private Node3D PlayerCameraPivot { get; set; }
	#endregion

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private AnimationTree animTree;
	private Timer idleTimer;

	public void TakeDamage(float amount)
	{
		if (IsDashing && DashTimer.TimeLeft / DashTimer.WaitTime >= 0.5)
			return;
		float oldHealth = Health;
		Health -= amount;
		EmitSignal(SignalName.HealthChanged, oldHealth, Health);
	}

	public void Heal(float amount)
	{
		float oldHealth = Health;
		Health += amount;
		EmitSignal(SignalName.HealthChanged, oldHealth, Health);
	}

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		PlayerCameraPivot = GetNode<Node3D>("PlayerCameraPivot");
		PlayerPivot = GetNode<Node3D>("Pivot");
		DashTimer = GetNode<Timer>("DashTimer");
		DashTimer.Timeout += OnDashTimerTimeout;
		DashCooldown = GetNode<Timer>("DashCooldown");
		Health = startingHealth;
		animTree = GetNode<AnimationTree>("AnimationTree");
		idleTimer = GetNode<Timer>("IdleTimer");
		idleTimer.Start();
		idleTimer.Timeout += _OnIdleTimerTimeout;
	}

	private void UpdateAnimationParameters(Vector2 inputDir)
	{
		if (IsDashing)
		{
			animTree.Set("parameters/conditions/idle", false);
			animTree.Set("parameters/conditions/dashing", true);
			animTree.Set("parameters/conditions/walking", false);
		}
		else
		{
			animTree.Set("parameters/conditions/dashing", false);
			if(inputDir != Vector2.Zero) {
				animTree.Set("parameters/conditions/idle", false);
				animTree.Set("parameters/conditions/walking", true);
			} else {
				animTree.Set("parameters/conditions/idle", true);
				animTree.Set("parameters/conditions/walking", false);
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		Vector3 camRot = PlayerCameraPivot.RotationDegrees;
		if (@event is InputEventMouseMotion mouseMotion)
		{
			camRot.Y -= mouseMotion.Relative.X * camSensitivity;
			camRot.X -= mouseMotion.Relative.Y * camSensitivity;
		}
		camRot.X = Mathf.Clamp(camRot.X, minCamPitch, maxCamPitch);
		PlayerCameraPivot.RotationDegrees = camRot;
	}

	public override void _Process(double delta)
	{
		if (direction != Vector3.Zero)
		{
			Vector3 bodyRotation = PlayerPivot.Rotation;
			bodyRotation.Y = Mathf.LerpAngle(bodyRotation.Y, Mathf.Atan2(-direction.X, -direction.Z), (float)delta * Speed);
			PlayerPivot.Rotation = bodyRotation;
		}
	}

	public void OnDashTimerTimeout()
	{
		DashCooldown.Start();
		IsDashing = false;
		idleTimer.Start();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (GlobalPosition.Y <= LowerBound )
		{
			if (Health > 0){
				TakeDamage(Health + 1);
				GD.Print("dead");
			}

			velocity.Y = 0;
			return;
		}

		if (!IsDashing)
		{
			#region verticality
			// Add the gravity.
			if (!IsOnFloor())
				velocity.Y -= gravity * (float)delta;
			else
			{
				JetPackStamina = 20.0f;
				HasDoubleJumped = false;
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("move_jump"))
			{
				if (IsOnFloor())
					velocity.Y = JumpVelocity;
				else if (!HasDoubleJumped)
				{
					velocity.Y = DoubleJumpForce;
					HasDoubleJumped = true;
				}
			}
			else if (Input.IsActionPressed("move_float") && !IsOnFloor() && JetPackStamina > 0)
			{
				velocity.Y += JetPackForce * (float)delta;
				JetPackStamina -= JetPackForce * (float)delta;
			}
			#endregion
			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
			direction = new Vector3(inputDir.X, 0, inputDir.Y).Rotated(Vector3.Up, PlayerCameraPivot.Rotation.Y).Normalized();
			if (inputDir != Vector2.Zero)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
				idleTimer.Stop();
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
				if(idleTimer.IsStopped())
					idleTimer.Start();
			}
			if (Input.IsActionJustPressed("move_dash") && DashCooldown.IsStopped())
			{
				IsDashing = true;
				DashTimer.Start();
				EmitSignal(SignalName.DashStart);
				velocity.Y = 0.0f;
				velocity.X = direction.X * DashSpeed;
				velocity.Z = direction.Z * DashSpeed;
				idleTimer.Stop();
			}
			UpdateAnimationParameters(inputDir);

		}
		else
		{
			Vector3 fwdVector = (-Transform.Basis.Z).Rotated(new Vector3(0, 1, 0), PlayerPivot.Rotation.Y);
			float timerProgress = Mathf.Clamp(Mathf.Pow((float)(DashTimer.TimeLeft / DashTimer.WaitTime), 4), 0.1f, 1.0f);

			float _speed = timerProgress * DashSpeed;
			velocity.X = fwdVector.X * _speed;
			velocity.Z = fwdVector.Z * _speed;
			UpdateAnimationParameters(Vector2.Zero);
		}
		Velocity = velocity;
		MoveAndSlide();
	}

	public void ResetAndReposition(Vector3 newPos)
	{
		GlobalPosition = newPos;
		Health = startingHealth;
		EmitSignal(SignalName.HealthChanged, 0, Health);
	}

	private void _OnIdleTimerTimeout() {
		animTree.Set("parameters/conditions/secondIdle", true);
	}
}
