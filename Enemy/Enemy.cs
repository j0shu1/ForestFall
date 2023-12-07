using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export]
	public HealthComponent HealthComponent;

	public static int TotalDamageTaken = 0;
	public static int MaxDamageTaken = 0;
    public static int Level = 1;
	public MovementComponent MovementComponent;

	public bool Attacking;
	public bool Dead;
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
		// Avoid granting additional exp to the players.
		if (Dead) return;

		// Inform EnemyMovement that the enemy can no longer move.
		Dead = true;

        // Play death animation.
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("Die");

		// Remove health bar.
		GetNode<TextureProgressBar>("HealthBar").Visible = false;

		// Disable attack radius.
		GetNode<CollisionShape2D>("AttackRange/CollisionShape2D").Disabled = true;

		// Give the player money and experience.
        int exp = 13;
		Main.Hud.AddExp(exp);
		GetTree().CallGroup("Player", "AddMoney", GD.RandRange(5, 10));
    }

	public void LevelUp()
	{
		if (Dead) return;

		HealthComponent.LevelUp();
		GetNode<CpuParticles2D>("LevelUpParticles").Emitting = true;
	}

	private void OnAttackRangeBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			_playerTarget = player;

			if (!Attacking)
				AttackToward(player.GlobalPosition);
		}
	}
    
	private void OnAttackRangeBodyExited(Node2D body)
    {
        if (body == _playerTarget)
        {
            _playerTarget = null;
        }
    }

    private async void OnAnimatedSprite2dAnimationFinished()
	{
		// When the animation completes, if there is a _targetPlayer, attack them.
		// Otherwise, indicate that attacking is over.
		var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        switch (sprite.Animation)
        {
            case "Attack":
                // Play idle animation for a moment.
                sprite.Play("Idle");
                await ToSignal(GetTree().CreateTimer(0.25), SceneTreeTimer.SignalName.Timeout);

                if (_playerTarget is null)
                {
                    Attacking = false;
                }
                else
                {
                    AttackToward(_playerTarget.GlobalPosition);
                }
                break;
            case "Die":
				SetCollisionLayerValue(1, false);
				SetCollisionMaskValue(1, false);

                await ToSignal(GetTree().CreateTimer(5), SceneTreeTimer.SignalName.Timeout);

				Tween tween = CreateTween();
				tween.TweenProperty(sprite, "modulate", new Color(1, 1, 1, 0), 1);

                await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
                QueueFree();
                break;
            default:
                break;
        }
    }

    private void AttackToward(Vector2 targetPosition)
	{
		var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("Attack");

		Node2D attackRotation = GetNode<Node2D>("AttackRotation");
		CollisionShape2D weaponCollisionShape = GetNode<CollisionShape2D>("AttackRotation/WeaponArea/WeaponCollisionShape");
		FacingRight = targetPosition.X > GlobalPosition.X;

        Tween tween = CreateTween().SetParallel(false);
		double attackTime = 0.25;

		Attacking = true;
		tween.TweenProperty(weaponCollisionShape, "disabled", false, 0);
		tween.TweenProperty(attackRotation, "rotation_degrees", FacingRight ? 90 : -270, attackTime);
		tween.TweenProperty(weaponCollisionShape, "disabled", true, 0);
        attackRotation.RotationDegrees = -90;

		GetNode<AudioStreamPlayer2D>("AttackSound").Play();
	}

	private void OnWeaponAreaBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			player.Hurt(3 + 3 * Level);
		}
	}

	private void OnGetNextPathTimeout()
	{
		// If there is no player in our attack range, move toward them.
        if (_playerTarget is null && !Attacking)
        {
			TargetClosestPlayer();
        }
		else
		{
			// If there is a player in our attack range, don't move.
			GetNode<NavigationAgent2D>("NavigationAgent2D").TargetPosition = GlobalPosition;
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
			GetNode<NavigationAgent2D>("NavigationAgent2D").TargetPosition = target.Position;
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
		GetNode<NavigationAgent2D>("NavigationAgent2D").TargetPosition = targetPlayer.GlobalPosition;
	}

	private void OnDespawnTimerTimeout()
	{
		QueueFree();
	}

	private void OnVisibleOnScreenNotifier2dScreenExited()
	{
		GetNode<Timer>("DespawnTimer").Start();
	}

	private void OnVisibleOnScreenNotifier2dScreenEntered()
	{
        GetNode<Timer>("DespawnTimer").Stop();
	}
}
