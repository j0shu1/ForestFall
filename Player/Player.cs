using Godot;
using Godot.Collections;

public partial class Player : CharacterBody2D
{
	public MovementComponent MovementComponent;
    public WeaponComponent WeaponComponent;
    public HealthComponent HealthComponent;
    public bool facingRight = true;

    private Dictionary<Item.ItemType, int> _inventory = new();

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
	{
		MovementComponent.Move(this, delta);

        // Attack, if applicable.
        if (Input.IsActionPressed("attack"))
            WeaponComponent.Attack();

        MoveAndSlide();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            WeaponComponent.LookAt(GetGlobalMousePosition());
            HandleWeaponSpriteDirection();
        }

        if (@event is InputEventJoypadMotion)
        {
            AimWeaponWithController();
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

	private void AimWeaponWithController()
	{
        // Look at the direction the right stick is moved.
        var rightStick = new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
        if (rightStick.Abs().X > 0.05 || rightStick.Abs().Y > 0.05)
        {
            WeaponComponent.LookAt(WeaponComponent.GlobalPosition + rightStick);
        }
        else
        {
            WeaponComponent.RotationDegrees = facingRight ? 0 : 180;
        }

        HandleWeaponSpriteDirection();
    }

    private void HandleWeaponSpriteDirection()
    {
        WeaponComponent.RotationDegrees %= 360;
        WeaponComponent.GetNode<Sprite2D>("Sprite2D").FlipV = WeaponComponent.RotationDegrees < -90 || WeaponComponent.RotationDegrees > 90;
    }
}