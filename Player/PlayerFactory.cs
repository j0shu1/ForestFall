using Godot;

public static class PlayerFactory
{
    private static readonly PackedScene _playerScene = (PackedScene)ResourceLoader.Load("res://Player/player.tscn");
    private static readonly PackedScene _handgunScene = (PackedScene)ResourceLoader.Load("res://Player/Weapons/handgun.tscn");
    private static readonly PackedScene _shotgunScene = (PackedScene)ResourceLoader.Load("res://Player/Weapons/shotgun.tscn");

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
        WeaponComponent weaponComponent = null;
        player.HealthComponent.HealthBar = Main.Hud.GetNode<TextureProgressBar>("HealthBar");

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

        // Attach applicable weapon to player.
        switch (weaponType)
        {
            case EntityType.Handgunner:
                weaponComponent = _handgunScene.Instantiate<WeaponComponent>();
                player.MovementComponent.Speed = 400.0f;
                player.HealthComponent.SetInitialMaxHealth(33);
                break;
            case EntityType.Shotgunner:
                weaponComponent = _shotgunScene.Instantiate<WeaponComponent>();
                player.MovementComponent.Speed = 300.0f;
                player.HealthComponent.SetInitialMaxHealth(31);
                player.HealthComponent.SetHealthIncreaseMultiplier(0.01f);
                player.HealthComponent.SetLinearHealthIncrease(3);
                break;
        }

        player.WeaponComponent = weaponComponent;
        player.AddChild(weaponComponent);



        player.GlobalPosition = location;

        return player;
    }
}
