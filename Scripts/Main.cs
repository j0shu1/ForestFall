using Godot;

public partial class Main : Node
{
	public static Hud Hud;
	[Export]
	private Marker2D _playerSpawnLocation;

	private const float TOP_OF_SCENE = 0;
	private const float BOTTOM_OF_SCENE = 1200;
	private const int MAX_ENEMIES = 5;
	private const short CHEST_COUNT = 2;
	private const int GAME_X_MIN = -1000;
	private const int GAME_X_MAX = 1000;
	private static PackedScene _chestScene = (PackedScene)ResourceLoader.Load("res://Chest/chest.tscn");

	private Player _player;
	private RayCast2D _ray;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Add a raycast for the purposes of spawning items on the ground.
		Hud = GetNode<Hud>("HUD");
		_ray = new RayCast2D();
		_ray.SetCollisionMaskValue(1, false);
		_ray.SetCollisionMaskValue(3, true);
		AddChild(_ray);
		
        // Spawn the player.
        Player player = PlayerFactory.CreatePlayer(
			PlayerFactory.EntityType.Shotgunner,
			_playerSpawnLocation.GlobalPosition);
		AddChild(player);

        _player = player;

		// Attach the camera to the player.
		AddPlayerCamera(player);

		SpawnChests();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
		if (@event is InputEventKey inputEventKey)
			if (inputEventKey.IsActionPressed("ui_cancel"))
				GetTree().Quit();
    }

    private static void AddPlayerCamera(Player player)
	{
        Camera2D playerCamera = new()
        {
            PositionSmoothingEnabled = true,
            PositionSmoothingSpeed = 4f,
            Zoom = new Vector2(0.75f, 0.75f)
        };
        player.AddChild(playerCamera);
    }

	private void OnSpawnTimerTimeout()
	{
		if (GetTree().GetNodesInGroup("Enemy").Count < MAX_ENEMIES)
        {
			CreateEnemyAtRandomLocation();
            //SpawnChests();
        }
	}

	private void SpawnChests()
	{
        GD.Print("Ran SpawnChests()");
        RayCast2D ray = _ray;

        // Determine the random X value.
        float targetX = 123.0f;

        ray.GlobalPosition = new Vector2(targetX, TOP_OF_SCENE);
        ray.TargetPosition = new Vector2(targetX, BOTTOM_OF_SCENE);

        if (ray.GetCollider() is TileMap)
        {
            GD.Print($"Created chest @ {ray.GetCollisionPoint()}");
            Chest chest = _chestScene.Instantiate<Chest>();
            chest.GlobalPosition = ray.GetCollisionPoint();
            AddChild(chest);
        }

        //await ToSignal(GetTree().CreateTimer(2.5), SceneTreeTimer.SignalName.Timeout);
        //GD.Print("Ran spawn chests.");

        //      RayCast2D ray = new();
        //      ray.SetCollisionMaskValue(1, false);
        //      ray.SetCollisionMaskValue(3, true);
        //      AddChild(ray);


        //      for (int i = 0; i <  CHEST_COUNT; i++)
        //{
        //	float targetX = GD.RandRange(GAME_X_MIN, GAME_X_MAX);
        //	GD.Print($"Random x is {targetX}");            

        //          ray.GlobalPosition = new Vector2(targetX, TOP_OF_SCENE);
        //	ray.TargetPosition = new Vector2(targetX, BOTTOM_OF_SCENE);
        //	GD.Print($"({ray.GlobalPosition.X}, {ray.GlobalPosition.Y}) -> ({ray.TargetPosition.X}, {ray.TargetPosition.Y})");
        //	GD.Print($"CollisionPoint: {ray.GetCollisionPoint()}");
        //	if (ray.GetCollider() is TileMap)
        //	{
        //		GD.Print("Ray collided with TileMap!");
        //		Chest chest = _chestScene.Instantiate<Chest>();
        //		chest.GlobalPosition = ray.GetCollisionPoint() + new Vector2(0, -20);
        //		GD.Print($"Spawned chest at {ray.GetCollisionPoint()}");
        //		AddChild(chest);
        //	}

        //	ray.QueueFree();
        //}
    }

    private void CreateEnemyAtRandomLocation()
	{
		RayCast2D ray = _ray;

		// Determine the random X value.
		float targetX = _player.GlobalPosition.X;
		targetX += GD.RandRange(-100, 100);
		targetX += GD.RandRange(0, 1) == 0 ? -300 : 300;

        ray.GlobalPosition = new Vector2(targetX, TOP_OF_SCENE);
        ray.TargetPosition = new Vector2(targetX, BOTTOM_OF_SCENE);

        // TODO: The following must be adjusted.
        // There needs to be a collision layer specifically for the tilemap
        // which the players and enemies will collide with. This way, a ray
        // can only collide with the tilemap physics layer, enabling a new
        // enemy to spawn under another enemy, instead of continually looking
        // for an open view from the ceiling of the world to the bottom of it.
        if (ray.GetCollider() is TileMap)
        {
            //GD.Print("Created enemy");
            Enemy enemy = EnemyFactory.CreateEnemy(
                EnemyFactory.EnemyType.Minion,
                location: ray.GetCollisionPoint());
			AddChild(enemy);
		}
	}

	private RayCast2D CreateVerticalRay()
	{
        // Create the spawn raycast.
        RayCast2D ray = new();
		
        // Remove default collision mask and add a collision mask for just the tilemap.
        ray.SetCollisionMaskValue(1, false);
		ray.SetCollisionMaskValue(3, true);

        AddChild(ray);
		return ray;
    }
}
