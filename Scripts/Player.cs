using Godot;
using Godot.Collections;

public partial class Player : CharacterBody2D
{
	public MovementComponent MovementComponent;
    public bool facingRight = true;

    private StringName _attack = new("Attack");
	private bool _controllerConnected;
	private Node2D _weapon;
    private Variant[] _args;
    private Dictionary<string, int> _items = new();
    
    public override void _Ready()
    {
        _args = new Variant[] { this };
		_weapon = GetNode<Node2D>("Weapon");
        _controllerConnected = Input.GetConnectedJoypads().Count > 0;
    }

    public override void _Process(double delta)
    {
        if (_controllerConnected)
        {
            AimWeaponWithController();
        }

        HandleWeaponSpriteDirection();
    }

    public override void _PhysicsProcess(double delta)
	{
		MovementComponent.Move(this, delta);

        // Attack, if applicable.
        if (Input.IsActionPressed("attack"))
            _weapon.Call(_attack, _args);

        MoveAndSlide();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            _weapon.LookAt(GetGlobalMousePosition());
        }
    }

    public void AddItem(string itemName)
    {
        if (_items.ContainsKey(itemName))
            _items[itemName]++;
        else
            _items.Add(itemName, 1);

        GD.Print($"Player now has {_items[itemName]} {itemName}");
    }

    public int ItemCount(string request)
    {
        return _items.ContainsKey(request) ? _items[request] : 0;
    }

	private void AimWeaponWithController()
	{
        // Look at the direction the right stick is moved.
        var rightStick = new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
        if (rightStick.Abs().X > 0.05 || rightStick.Abs().Y > 0.05)
        {
            _weapon.LookAt(_weapon.GlobalPosition + rightStick);
        }
        else
        {
            _weapon.RotationDegrees = facingRight ? 0 : 180;
        }

        HandleWeaponSpriteDirection();
    }

    private void HandleWeaponSpriteDirection()
    {
        _weapon.RotationDegrees %= 360;
        _weapon.GetNode<Sprite2D>("Sprite2D").FlipV = _weapon.RotationDegrees < -90 || _weapon.RotationDegrees > 90;
    }
}