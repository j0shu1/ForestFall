using Godot;

public abstract partial class WeaponComponent : Node2D
{
    protected Player _player;
    protected bool _attackEnabled = true;
    protected float _attackCooldown = 20f;
    public abstract void Attack();


    public void AimWeaponWithController()
    {
        // Look at the direction the right stick is moved.
        var rightStick = new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
        if (rightStick.Abs().X > 0.05 || rightStick.Abs().Y > 0.05)
        {
            LookAt(_player.GlobalPosition + rightStick);
        }
        else
        {
            RotationDegrees = _player.FacingRight ? 0 : 180;
        }

        HandleWeaponSpriteDirection();
    }

    public void HandleWeaponSpriteDirection()
    {
        RotationDegrees %= 360;
        GetNode<Sprite2D>("Sprite2D").FlipV = RotationDegrees < -90 || RotationDegrees > 90;
    }
}
