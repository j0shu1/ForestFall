using Godot;

public abstract partial class Gun : WeaponComponent
{
    [Export]
    protected Marker2D _bulletSpawnLocation;
    [Export]
    protected PackedScene _bulletScene;

    protected int _magazineCapacity;
    protected int _shots;
    protected float _reloadTime;
    protected TextureProgressBar _magazine;

    public override void Attack()
    {
        Shoot();
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionPressed("reload"))
        {
            Reload();
        }
    }

    public abstract void Shoot();
    protected virtual async void Reload()
    {
        if (!_attackEnabled) return; // Let whatever disabled shooting enable it again.

        _attackEnabled = false;
        await ToSignal(GetTree().CreateTimer(_reloadTime), SceneTreeTimer.SignalName.Timeout);
        _shots = _magazineCapacity;
        Main.Hud.SetBullets(_magazineCapacity);
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