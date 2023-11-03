using Godot;

public abstract partial class WeaponComponent : Node2D
{
    protected Player _player;
    protected bool _attackEnabled = true;
    protected float _attackCooldown = 20f;
    public abstract void Attack();
}
