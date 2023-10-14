using Godot;

public partial class Handgun : Gun
{
    public override void _Ready()
    {
        _magazineCapacity = 6;
        _shots = _magazineCapacity;
        _attackCooldown = 0.5f;
        _reloadTime = 1.0f;

        _weaponScene = (PackedScene)ResourceLoader.Load("res://Scenes/handgun.tscn");
        _bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet.tscn");

        _bulletSpawnLocation = GetNode<Sprite2D>("Sprite2D").GetNode<Marker2D>("BulletSpawnLocation");
    }

    public override void Shoot(Player player)
    {
        if (!_attackEnabled) return;

        var random_offset = new Vector2(GD.RandRange(-5, 5), GD.RandRange(-5, 5));

        // This will work for now, but needs to be replaced with a raycast.
        Bullet bullet = _bulletScene.Instantiate<Bullet>();
        bullet.Damage = 5;
        bullet.Position = _bulletSpawnLocation.GlobalPosition;
        bullet.Velocity = bullet.Position - player.Position + random_offset;
        player.GetParent().AddChild(bullet);

        if (--_shots <= 0)
            Reload();
        else
            Recoil();
    }
}
