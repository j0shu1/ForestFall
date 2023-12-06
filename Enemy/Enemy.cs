using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export]
	public HealthComponent HealthComponent;
	[Export]
	public NavigationAgent2D NavigationAgent;
	[Export]
	private Timer _despawnTimer;
	[Export]
	private Timer _attackTimer;
	[Export]
	private Timer _attackCooldownTimer;

	public static int TotalDamageTaken = 0;
	public static int MaxDamageTaken = 0;
    public static int Level = 1;
	public MovementComponent MovementComponent;

	public bool Attacking;
	public bool Dead = false;
	public bool FacingRight = true;

	private Player _playerTarget;
	private int _maxHealth;

    public override void _PhysicsProcess(double delta)
	{
		MovementComponent.Move(this, delta);
    }

    public void Hurt(int amount)
	{
		HealthComponent.Hurt(amount);
		TotalDamageTaken += amount;

		if (amount > MaxDamageTaken)
		{
			MaxDamageTaken = amount;
		}
	}

	public void Die()
	{
		// Inform MovementComponent that it is no longer able to move.
		Dead = true;
		var enemySprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        enemySprite.Play("Die");
        enemySprite.AnimationLooped += () =>
        {
            QueueFree();
        };

        // Disable hitbox and movement.
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
		MovementComponent = new NoMovement();

		// Give the player money and experience.
        int exp = 13;
		Main.Hud.AddExp(exp);
		GetTree().CallGroup("Player", "AddMoney", GD.RandRange(5, 10));
    }

	public void LevelUp()
	{
		// TODO: Produce level up particles.
		HealthComponent.LevelUp();
		GetNode<CpuParticles2D>("LevelUpParticles").Emitting = true;
	}

    private void OnAttackAreaBodyEntered(Node2D body)
	{
		if (Dead) return;
		if (body is Player player)
		{
			_playerTarget = player;
			_attackTimer.Start();
			Attacking = true;
			_attackCooldownTimer.Start();			

			AttackToward(player.GlobalPosition);
		}
	}

	private async void AttackToward(Vector2 targetPosition)
	{
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("Attack");

		Node2D attackRotation = GetNode<Node2D>("AttackRotation");
		CollisionShape2D attackHitBox = GetNode<CollisionShape2D>("AttackRotation/AttackingArea/AttackHitBox");
		//ColorRect colorRect = GetNode<ColorRect>("AttackRotation/AttackingArea/ColorRect");
		bool isLeft = targetPosition >= GlobalPosition;

		Tween tween = CreateTween().SetParallel(false);
		double attackTime = 0.25;

		Attacking = true;
		tween.TweenProperty(attackHitBox, "disabled", false, 0);
		//tween.TweenProperty(colorRect, "visible", true, 0);
		tween.TweenProperty(attackRotation, "rotation_degrees", isLeft ? 90 : -270, attackTime);
		tween.TweenProperty(attackHitBox, "disabled", true, 0);
		//tween.TweenProperty(colorRect, "visible", false, 0);
        attackRotation.RotationDegrees = -90;

		GetNode<AudioStreamPlayer2D>("AttackSound").Play();

        await ToSignal(GetTree().CreateTimer(attackTime * 3), SceneTreeTimer.SignalName.Timeout);
		Attacking = false;
	}

	private void OnAttackingAreaBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			player.Hurt(3 + 3 * Level);
		}
	}


    private void OnAttackAreaBodyExited(Node2D body)
	{
		if (body == _playerTarget)
		{
			_playerTarget = null;
			_attackTimer.Stop();
		}
	}

    private void OnAttackTimerTimeout()
	{
		// If the player is not null, hurt them.
		//_playerTarget?.Hurt(3 + 3 * Level);
	}

	private void OnAttackCooldownTimeout()
	{
		Attacking = false;
	}

	private void OnGetNextPathTimeout()
	{
		// If there is no player in our attack range, move toward them.
        if (_playerTarget is null && !Attacking)
        {
			TargetClosestPlayer();
			//NavigationAgent.TargetPosition = Player.GlobalPosition;
        }
		else
		{
			// If there is a player in our attack range, don't move.
			NavigationAgent.TargetPosition = GlobalPosition;
		}
	}

	private void TargetClosestPlayer()
	{
		// Get the players from the tree.
		var players = GetTree().GetNodesInGroup("Player");

		// If there is only one player, target them.
		if (players.Count == 1)
		{
			Player target = (Player)players[0];
			NavigationAgent.TargetPosition = target.Position;
			return;
		}

		// Otherwise, target the closest player instead.
		double closestDistance = 0;
		Player targetPlayer = (Player)players[0];
		foreach (var player in players)
		{
			// Cast the player as a Player object.
			Player currPlayer = (Player)player;

			Vector2 location = currPlayer.GlobalPosition;
			Vector2 difference = GlobalPosition - location;

			// Find the total distance to this player.
			double totalDistance = Mathf.Sqrt(difference.X * difference.X + difference.Y + difference.Y);

			if (totalDistance < closestDistance)
			{
				closestDistance = totalDistance;
				targetPlayer = currPlayer;
			}
		}

		// Once the closest player has been determined, target them.
		NavigationAgent.TargetPosition = targetPlayer.GlobalPosition;
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
