using Godot;

public partial class Main : Node
{
	public static Hud Hud;

	private const float SCENE_Y_MIN = -500;
	private const float SCENE_Y_MAX = 1200;
	private const int SCENE_X_MIN = -19572;
	private const int SCENE_X_MAX = 24181;
	private const int MAX_ENEMIES = 50;
	private const int CHESTS_TO_SPAWN = 16;
	private static PackedScene _chestScene = (PackedScene)ResourceLoader.Load("res://Chest/chest.tscn");

	private Player _player;
	private RayCast2D _ray;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Create a raycast for the purposes of spawning things on the ground.
		Hud = GetNode<Hud>("HUD");
		_ray = new RayCast2D();
		_ray.SetCollisionMaskValue(1, false);
		_ray.SetCollisionMaskValue(3, true);
		AddChild(_ray);
		
        // Spawn the player.
        Player player = PlayerFactory.CreatePlayer(
			PlayerFactory.EntityType.Shotgunner,
			GetNode<Marker2D>("PlayerSpawnLocation").GlobalPosition);
		AddChild(player);

        _player = player;

		// Attach the camera to the player.
		AddPlayerCamera(player);

        // Set up the chests.
        Chest.SetChestCost(37);
		SpawnChests();
    }

    private static void AddPlayerCamera(Player player)
	{
        Camera2D playerCamera = new()
        {
            PositionSmoothingEnabled = true,
            PositionSmoothingSpeed = 5f,
            Zoom = new Vector2(0.5f, 0.5f)
        };
        player.AddChild(playerCamera);
    }

	private async void SpawnChests()
	{
		// Wait for the TileMap to exist.
        await ToSignal(GetTree().CreateTimer(0.00001), SceneTreeTimer.SignalName.Timeout);

		// Calculate equal distances to spawn each chest from one another.
		int distanceIncrement = (SCENE_X_MAX - SCENE_X_MIN) / CHESTS_TO_SPAWN;

		// Spawn the chests.
		for (int i = 0; i < CHESTS_TO_SPAWN; i++)
		{
			// Spawn a chest from the SCENE_X_MIN, shifting by distanceIncrement all the way to SCENE_X_MAX.
			SpawnChest(SCENE_X_MIN + i * distanceIncrement, SCENE_X_MIN + (i + 1) * distanceIncrement);
		}
	}

	private void SpawnChest(int min, int max, int attempts = 0)
	{
		// Stop trying after too many attempts.
		if (attempts >= 3) return;

        float targetX = GD.RandRange(min, max);
		_ray.GlobalPosition = new Vector2(targetX, SCENE_Y_MIN);
		_ray.TargetPosition = new Vector2(0, SCENE_Y_MAX);

		_ray.ForceRaycastUpdate();

		if (_ray.GetCollider() is TileMap)
		{
			Chest chest = _chestScene.Instantiate<Chest>();
			chest.GlobalPosition = _ray.GetCollisionPoint() - new Vector2(0, 32);
			GetNode<Node>("Chests").AddChild(chest);
		}
		else
		{
			// If we hit a chest or something other than the TileMap, try again.
			SpawnChest(min, max, attempts + 1);
		}
	}

	private void OnSpawnTimerTimeout()
	{
		if (GetTree().GetNodesInGroup("Enemy").Count < MAX_ENEMIES)
        {
			CreateEnemyAtRandomLocation();
        }
	}

    private void CreateEnemyAtRandomLocation()
	{
		// Determine the random X value.
		float targetX = _player.GlobalPosition.X;
		targetX += GD.RandRange(-400, 400);
		targetX += GD.RandRange(0, 1) == 0 ? 600 : -600;

		_ray.GlobalPosition = new Vector2(targetX, SCENE_Y_MIN);
		_ray.TargetPosition = new Vector2(0, SCENE_Y_MAX);

		_ray.ForceUpdateTransform();
		_ray.ForceRaycastUpdate();

        // TODO: The following must be adjusted.
        // There needs to be a collision layer specifically for the tilemap
        // which the players and enemies will collide with. This way, a ray
        // can only collide with the tilemap physics layer, enabling a new
        // enemy to spawn under another enemy, instead of continually looking
        // for an open view from the ceiling of the world to the bottom of it.
        if (_ray.GetCollider() is TileMap || _ray.GetCollider() is Chest)
        {
            //GD.Print("Created enemy");
            Enemy enemy = EnemyFactory.CreateEnemy(
                enemyType: EnemyFactory.EnemyType.Minion,
                location: _ray.GetCollisionPoint());
			AddChild(enemy);
		}
	}
}
