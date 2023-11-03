using Godot;

public partial class Handgun : Gun
{
    private const float ATTACK_COOLDOWN_BASE = 0.5f;
    public override void _Ready()
    {
        _player = GetParent<Player>();
        _magazineCapacity = 6;
        _shots = _magazineCapacity;
        _attackCooldown = ATTACK_COOLDOWN_BASE;
        _reloadTime = 1.0f;

        _weaponScene = (PackedScene)ResourceLoader.Load("res://Player/Weapons/handgun.tscn");
        _bulletScene = (PackedScene)ResourceLoader.Load("res://Bullet/bullet.tscn");

        _bulletSpawnLocation = GetNode<Sprite2D>("Sprite2D").GetNode<Marker2D>("BulletSpawnLocation");
    }

    public override void Shoot()
    {
        // Make sure we are using the correct attack cooldown.
        _attackCooldown = ATTACK_COOLDOWN_BASE - (0.1f * _player.GetItemCount(Item.ItemType.AttackSpeed));

        if (!_attackEnabled) return;

        int critCount = _player.GetItemCount(Item.ItemType.CritChance);
        bool crit = true;

        if (critCount < 10)
        {
            crit = GD.RandRange(1, 100) <= 10 * critCount;
        }

        //var random_offset = new Vector2(GD.RandRange(-1, 1), GD.RandRange(-1, 1));

        // This will work for now, but needs to be replaced with a raycast.
        Bullet bullet = _bulletScene.Instantiate<Bullet>();

        bullet.Damage = 5;
        bullet.Crit = crit;
        bullet.Piercing = true;

        bullet.Position = _bulletSpawnLocation.GlobalPosition;
        bullet.Velocity = bullet.Position - _player.Position;// + random_offset;
        _player.GetParent().AddChild(bullet);

        if (--_shots <= 0)
            Reload();
        else
            Recoil();
    }
}
