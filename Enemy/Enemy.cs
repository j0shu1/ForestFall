using Godot;
using System.Net.Security;

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

	public static int Level = 1;
	public MovementComponent MovementComponent;

	private Player _playerTarget;

    public override void _PhysicsProcess(double delta)
	{
		MovementComponent.Move(this, delta);
    }
    public void Hurt(int amount)
	{
		HealthComponent.Hurt(amount);
	}

	private void OnAttackAreaBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			_playerTarget = player;
			_attackTimer.Start();
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
		_playerTarget?.Hurt(5);
	}

	private void OnGetNextPathTimeout()
	{
		// If there is no player in our attack range, move toward them.
        if (_playerTarget is null)
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
