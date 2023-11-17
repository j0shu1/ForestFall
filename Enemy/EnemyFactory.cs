using Godot;

public static class EnemyFactory
{
    private static readonly PackedScene _minionScene = (PackedScene)ResourceLoader.Load("res://Enemy/enemy.tscn");

    public enum EnemyType
    {
        Minion,
        ShieldBasher
    }

    public enum MovementType
    {
        Default,
        None
    }

    public static Enemy CreateEnemy(EnemyType enemyType, Vector2 location, MovementType movementType = MovementType.Default)
    {
        Enemy enemy = _minionScene.Instantiate<Enemy>();
        enemy.GlobalPosition = location;
        
        switch (movementType)
        {
            case MovementType.Default:
                enemy.MovementComponent = new EnemyMovement();
                break;
            case MovementType.None:
                enemy.MovementComponent = new NoMovement();
                break;
        }

        enemy.MovementComponent.Speed = 200.0f;
        enemy.HealthComponent.SetInitialMaxHealth(Enemy.Level * 10);

        return enemy;
    }
}
