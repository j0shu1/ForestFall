using Godot;

public static class PlayerFactory
{
    private static PackedScene _playerScene = (PackedScene)ResourceLoader.Load("res://Scenes/player.tscn");
    private static PackedScene _handgunScene = (PackedScene)ResourceLoader.Load("res://Scenes/handgun.tscn");
    private static PackedScene _shotgunScene = (PackedScene)ResourceLoader.Load("res://Scenes/shotgun.tscn");

    public enum EntityType
    {
        Handgunner,
        Shotgunner
    }
    
    public enum MovementType
    {
        Default,
        None
    }

    public static Player CreatePlayer(EntityType weaponType, Vector2 location, MovementType movementType = MovementType.Default)
    {
        // Create player of type.
        Player player = _playerScene.Instantiate<Player>();

        // Attach applicable weapon to player.
        switch (weaponType)
        {
            case EntityType.Handgunner:
                player.AddChild(_handgunScene.Instantiate<Node2D>());
                break;
            case EntityType.Shotgunner:
                player.AddChild(_shotgunScene.Instantiate<Node2D>());
                break;
        }

        // Attach movement to player.
        switch (movementType)
        {
            case MovementType.Default:
                player.MovementComponent = new PlayerMovement();
                break;
            case MovementType.None:
                player.MovementComponent = new NoMovement();
                break;
        }

        player.GlobalPosition = location;

        return player;
    }
}
