using Godot;

public partial class Enemy : CharacterBody2D
{
	private int _health = 10;
	private NavigationAgent2D _navigationAgent;
	private static Player _player;

	public const float Speed = 20.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_player = GetParent().GetNode<Player>("Player");
		_navigationAgent.TargetPosition = _player.GlobalPosition;
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		_navigationAgent.TargetPosition = _player.GlobalPosition;

		if (!_navigationAgent.IsTargetReached())
		{
			Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

			int movementDirection = nextPathPosition.X > _player.GlobalPosition.X ? -1 : 1;
			velocity.X = movementDirection * Speed;
		}

		_navigationAgent.Velocity = velocity;

		Velocity = velocity;
		MoveAndSlide();
	}

	private void Damage(int amount)
	{
		_health -= amount;

		if (_health <= 0)
		{
			QueueFree();
		}
	}

}
