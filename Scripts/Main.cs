using Godot;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Spawn the player.
		Player player = PlayerFactory.CreatePlayer(
			PlayerFactory.EntityType.Shotgunner,
			GetNode<Marker2D>("PlayerSpawnLocation").GlobalPosition);
		AddChild(player);

		// Spawn the enemy.
		Enemy enemy = EnemyFactory.CreateEnemy(
			EnemyFactory.EnemyType.Minion,
			GetNode<Marker2D>("EnemySpawnLocation").GlobalPosition);
		AddChild(enemy);
	}

    public override void _UnhandledKeyInput(InputEvent @event)
    {
		if (@event is InputEventKey inputEventKey)
			if (inputEventKey.IsActionPressed("ui_cancel"))
				GetTree().Quit();
    }
}
