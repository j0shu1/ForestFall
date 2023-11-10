using Godot;
using Godot.Collections;

public partial class Player : CharacterBody2D
{
	public MovementComponent MovementComponent;
    public WeaponComponent WeaponComponent;
    public HealthComponent HealthComponent;
    public bool FacingRight = true;

    private Dictionary<Item.ItemType, int> _inventory = new();

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
}