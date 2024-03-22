using Godot;
using Vector3 = Godot.Vector3;
using Vector2 = Godot.Vector2;

public partial class Player : CharacterBody3D
{
	[Export]
	private float Health = 10.0f;
	[Signal]
	public delegate void HealthChangedEventHandler(float oldValue, float newValue);
	[Export]
	public const float Speed = 5.0f;
#region jumping-jetpack
	[Export]
	public const float JumpVelocity = 4.5f;
	[Export]
	public const float DoubleJumpForce = 6.5f;
	[Export]
	public float JetPackStamina = 50.0f;
	[Export]
	public float JetPackForce = 1.0f;
	private bool HasDoubleJumped = false;
#endregion
#region dashing
	public bool IsDashing = false;
	[Export]
	public const float DashSpeed = 10.0f;
	private Timer DashTimer;
#endregion
	private Node3D PlayerPivot {get; set;}

	private Vector3 direction = Vector3.Zero;

#region Camera
	[Export(PropertyHint.Range, "0.1,1.0")] public float camSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] float minCamPitch = -50f;
	[Export(PropertyHint.Range, "0,90,1")] float maxCamPitch = 3;
	private Node3D PlayerCameraPivot {get; set;}
#endregion

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public void TakeDamage(float amount) {
		float oldHealth = Health;
		Health -= amount;
		EmitSignal(SignalName.HealthChanged, oldHealth, Health);
	}

	public void Heal(float amount) {
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
		DashTimer.Timeout += OnVoidTimerTimeout;
    }

    public override void _Input(InputEvent @event)
	{
		Vector3 camRot = PlayerCameraPivot.RotationDegrees;
		if(@event is InputEventMouseMotion mouseMotion) {
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
			bodyRotation.Y = Mathf.LerpAngle(bodyRotation.Y,Mathf.Atan2(-direction.X, -direction.Z), (float)delta * Speed);
			PlayerPivot.Rotation = bodyRotation;
		}
    }

	public void OnVoidTimerTimeout() {
		IsDashing = false;
	}

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if(!IsDashing) {
#region verticality
			// Add the gravity.
			if (!IsOnFloor())
				velocity.Y -= gravity * (float)delta;
			else {
				JetPackStamina = 50.0f;
				HasDoubleJumped = false;
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("move_jump")) {
				if(IsOnFloor())
					velocity.Y = JumpVelocity;
				else if(!HasDoubleJumped) {
					velocity.Y = DoubleJumpForce;
					HasDoubleJumped = true;	
				}
			}
			else if (Input.IsActionPressed("move_float") && !IsOnFloor() && JetPackStamina > 0) {
				velocity.Y += JetPackForce * (float)delta;
				JetPackStamina -= JetPackForce * (float)delta;
			}
#endregion
			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
			direction = new Vector3(inputDir.X, 0, inputDir.Y).Rotated(Vector3.Up, PlayerCameraPivot.Rotation.Y).Normalized();
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}
			if(Input.IsActionJustPressed("move_dash")) {
				IsDashing = true;
				DashTimer.Start();
				velocity.Y = 0.0f;
				velocity.X = direction.X * DashSpeed;
				velocity.Z = direction.Z * DashSpeed;
			}

		} else {
				Vector3 fwdVector = (-Transform.Basis.Z).Rotated(new Vector3(0, 1, 0), PlayerPivot.Rotation.Y);
				velocity.X = fwdVector.X * DashSpeed;
				velocity.Z = fwdVector.Z * DashSpeed;
		}
		Velocity = velocity;
		MoveAndSlide();
	}
}
