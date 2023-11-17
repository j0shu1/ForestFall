using Godot;
using Godot.Collections;

public partial class Player : CharacterBody2D
{
    [Export]
    public HealthComponent HealthComponent;
	public MovementComponent MovementComponent;
    public WeaponComponent WeaponComponent;
    public bool FacingRight = true;
    public static int Level = 1;

    private Dictionary<Item.ItemType, int> _inventory = new();
    private int _money;

    public override void _PhysicsProcess(double delta)
	{
		MovementComponent.Move(this, delta);
	}

    public override void _UnhandledInput(InputEvent @event)
    {
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

        GD.Print($"Player now has {_inventory[itemType]} {itemType}");
        // Display the collected item on the HUD.
        Main.Hud.DisplayCollectedItem(itemType);
    }

    public int GetItemCount(Item.ItemType request)
    {
        return _inventory.ContainsKey(request) ? _inventory[request] : 0;
    }

    public void Hurt(int damage)
    {
        HealthComponent.Hurt(damage);
    }

    public void AddMoney(int amount)
    {
        _money += amount;
        Main.Hud.UpdateMoney(_money);
    }

    public bool Purchase(int price)
    {
        if (price <= _money)
        {
            _money -= price;
            Main.Hud.UpdateMoney(_money);
            return true;
        }

        return false;
    }

    public void Die()
    {
        // Player death animation.
        // Change scene to game over.
        // Show statistics for the run.
    }

    public void LevelUp()
    {
        // TODO: Produce level up particles.
        HealthComponent.LevelUp();
    }
}