using Godot;

public partial class Shotgun : Gun
{
    private const int BULLETS_PER_SHOT = 6;

    public override void _Ready()
    {
        _magazineCapacity = 4;
        _shots = _magazineCapacity;
        _attackCooldown = 0.5f;
        _reloadTime = 1.5f;

        _weaponScene = (PackedScene)ResourceLoader.Load("res://Scenes/shotgun.tscn");
        _bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet.tscn");

        _bulletSpawnLocation = GetNode<Sprite2D>("Sprite2D").GetNode<Marker2D>("BulletSpawnLocation");
    }

    public override void Shoot(Player player)
    {
        if (!_attackEnabled) return;

        for (int i = 0; i < BULLETS_PER_SHOT; i++)
        {
            var random_offset = new Vector2(GD.RandRange(-20, 20), GD.RandRange(-20, 20));

            Bullet bullet = _bulletScene.Instantiate<Bullet>();
            bullet.Damage = 1;
            bullet.Position = _bulletSpawnLocation.GlobalPosition;
            bullet.Velocity = bullet.Position - player.Position + random_offset;

            // Kick the player away from the bullet velocity.
            player.Velocity -= new Vector2(x:bullet.Velocity.X, y:bullet.Velocity.Y);
            // Move the player after each bullet is fired.
            player.MoveAndSlide();

            player.GetParent().AddChild(bullet);
        }

        if (--_shots <= 0)
            Reload();
        else
            Recoil();


    }
}
