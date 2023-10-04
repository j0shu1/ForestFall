using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public PackedScene BulletScene;

	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		bool isOnFloor = IsOnFloor();

		// Add the gravity.
		if (!isOnFloor)
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if ( isOnFloor &&
			(Input.IsActionJustPressed("ui_accept")
			|| Input.IsActionJustPressed("move_up")
			|| Input.IsActionJustPressed("ui_up")))
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		direction += Input.GetVector("move_left", "move_right", "move_up", "move_down");
		direction = direction.Normalized();
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				Shoot();
			}
		}
    }

    private void Shoot()
	{
		Node2D mouseRotation = GetNode<Node2D>("MouseRotation");
		mouseRotation.LookAt(GetGlobalMousePosition());

		Marker2D marker = mouseRotation.GetNode<Marker2D>("Marker2D");

		Bullet bullet = BulletScene.Instantiate<Bullet>();
		bullet.Position = marker.GlobalPosition;
		bullet.Velocity = GetGlobalMousePosition() - bullet.Position;

        GetParent().AddChild(bullet);
	}
}
