using Godot;
using Godot.Collections;

public partial class Player : CharacterBody2D
{
    [Export]
    public HealthComponent HealthComponent;
	public MovementComponent MovementComponent;
    public WeaponComponent WeaponComponent;
    public bool FacingRight = true;
    public int DamageTaken = 0;
    public int AmountHealed = 0;
    public static int ExperienceGained = 0;
    public int GoldCollected = 0;
    public int GoldSpent = 0;
    public static int Level = 1;
    public bool Dead = false;

    private Dictionary<Item.ItemType, int> _inventory = new();
    private int _money;

    public override void _PhysicsProcess(double delta)
	{
		MovementComponent.Move(this, delta);
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Dead) return;
        if (Input.IsActionJustPressed("attack"))
            WeaponComponent.Attack();

        if (@event is InputEventMouseMotion)
        {
            WeaponComponent.LookAt(GetGlobalMousePosition());
            WeaponComponent.HandleWeaponSpriteDirection();
        }

        if (@event is InputEventJoypadMotion)
        {
            WeaponComponent.AimWeaponWithController();
        }
    }

    public void AddItem(Item.ItemType itemType)
    {
        if (_inventory.ContainsKey(itemType))
            _inventory[itemType]++;
        else
            _inventory.Add(itemType, 1);

        // Display the collected item on the HUD.
        Main.Hud.DisplayCollectedItem(itemType);
        Main.Hud.PushCollectedItemStatus($"Player collected [b]{GetItemCount(itemType)}x[/b] [color=#aaffaa][i]{Item.GetName(itemType)}[/i][/color].");
    }

    public int GetItemCount(Item.ItemType request)
    {
        return _inventory.ContainsKey(request) ? _inventory[request] : 0;
    }

    public void Hurt(int damage)
    {
        DamageTaken += damage;
        HealthComponent.Hurt(damage);
    }

    public void AddMoney(int amount)
    {
        _money += amount;
        GoldCollected += amount;
        Main.Hud.UpdateMoney(_money);
    }

    public bool Purchase(int price)
    {
        if (price <= _money)
        {
            _money -= price;
            GoldSpent += price;
            Main.Hud.UpdateMoney(_money);
            return true;
        }

        return false;
    }

    public void Die()
    {
        if (Dead) return;
        // Stop player animation.
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Stop();

        // Disable interaction.
        Dead = true;
        MovementComponent = new NoMovement();
        //GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

        // Hide HUD
        Main.Hud.Visible = false;

        // Display game over.
        PackedScene endScene = (PackedScene)ResourceLoader.Load("res://End Screen/end_screen.tscn");
        EndScreen endScreen = endScene.Instantiate<EndScreen>();

        // Show statistics for the run.
        endScreen.ShowEndScreen(player:this);
        GetParent().AddChild(endScreen);
    }

    public void LevelUp()
    {
        HealthComponent.LevelUp();
        GetNode<CpuParticles2D>("LevelUpParticles").Emitting = true;
    }
}