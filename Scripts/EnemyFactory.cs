using Godot;

public static class EnemyFactory
{
    private static PackedScene _minionScene = (PackedScene)ResourceLoader.Load("res://Scenes/enemy.tscn");

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

        return enemy;
    }
}
