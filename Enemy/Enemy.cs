using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export]
	private Timer _despawnTimer;

	public const float Speed = 200.0f;
	public static Player Player;
	public HealthComponent HealthComponent;

	private Player playerTarget;
	private NavigationAgent2D _navigationAgent;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_navigationAgent.TargetPosition = Player.GlobalPosition;
        HealthComponent = GetNode<HealthComponent>("HealthComponent");
    }

    public override void _PhysicsProcess(double delta)
	{
        var direction = ToLocal(_navigationAgent.GetNextPathPosition()).Normalized();
        direction *= Speed;
        Velocity = new Vector2(direction.X, Velocity.Y + (float)(gravity * delta));

        MoveAndSlide();
    }
    public void Hurt(int amount)
	{
		HealthComponent.Hurt(amount);
	}

	private void OnAttackAreaBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			playerTarget = player;
			AttackPlayerWhileInRange();
		}
	}

	private void OnAttackAreaBodyExited(Node2D body)
	{
		if (body == playerTarget)
		{
			playerTarget = null;
		}
	}

	private async void AttackPlayerWhileInRange()
	{
        while (playerTarget is not null)
		{
			playerTarget.Hurt(5);
			await ToSignal(GetTree().CreateTimer(2.5), SceneTreeTimer.SignalName.Timeout);
        }
    }

	private void OnGetNextPathTimeout()
	{
		_navigationAgent.TargetPosition = Player.GlobalPosition;
	}
	
	private void OnDespawnTimerTimeout()
	{
		// TODO: Disable collisions and movement, Play death animation.
		QueueFree();
	}

	private void OnVisibleOnScreenNotifier2dScreenExited()
	{
		_despawnTimer.Start();
	}

	private void OnVisibleOnScreenNotifier2dScreenEntered()
	{
		_despawnTimer.Stop();
	}

}
