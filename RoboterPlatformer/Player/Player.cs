using Godot;

public partial class Player : CharacterBody3D
{
	[Export]
	public float Health = 10.0f;
	[Export]
	public const float Speed = 5.0f;
	[Export]
	public const float JumpVelocity = 4.5f;

#region Camera
	[Export(PropertyHint.Range, "0.1,1.0")] public float camSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] float minCamPitch = -50f;
	[Export(PropertyHint.Range, "0,90,1")] float maxCamPitch = 3;
	private Node3D PlayerCameraPivot {get; set;}
#endregion

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		PlayerCameraPivot = GetNode<Node3D>("PlayerCameraPivot");
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

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("move_jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
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

		Velocity = velocity;
		MoveAndSlide();
	}
}
