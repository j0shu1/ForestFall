using Godot;

public abstract partial class Gun : WeaponComponent
{
    protected PackedScene _bulletScene;
    protected PackedScene _weaponScene;
    protected Marker2D _bulletSpawnLocation;
    protected int _magazineCapacity;
    protected int _shots;
    protected float _reloadTime;

    public override void Attack(Player player)
    {
        Shoot(player);
    }
    public abstract void Shoot(Player player);
    protected async void Reload()
    {
        if (!_attackEnabled) return; // Let whatever disabled shooting enable it again.

        _attackEnabled = false;
        await ToSignal(GetTree().CreateTimer(_reloadTime), SceneTreeTimer.SignalName.Timeout);
        _shots = _magazineCapacity;
        _attackEnabled = true;
    }

    protected async void Recoil()
    {
        if (!_attackEnabled) return; // Let whatever disabled shooting enable it again.

        _attackEnabled = false;
        await ToSignal(GetTree().CreateTimer(_attackCooldown), SceneTreeTimer.SignalName.Timeout);
        _attackEnabled = true;
    }
}