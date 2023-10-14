using Godot;

public partial class Enemy : CharacterBody2D
{
	private int _health = 10;
	private NavigationAgent2D _navigationAgent;
	private static Player _player;

	public const float Speed = 200.0f;

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
        var direction = ToLocal(_navigationAgent.GetNextPathPosition()).Normalized();
        direction *= Speed;
        Velocity = new Vector2(direction.X, Velocity.Y + (float)(gravity * delta));

        MoveAndSlide();
    }

	private void OnGetNextPathTimeout()
	{
		_navigationAgent.TargetPosition = _player.GlobalPosition;
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
