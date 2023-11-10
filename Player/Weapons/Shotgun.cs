using Godot;

public partial class Shotgun : Gun
{
    [Export]
    private CpuParticles2D _shotBurst;
    [Export]
    private Timer _reloadTimer;

    private const int BULLETS_PER_SHOT = 6;
    private const float ATTACK_COOLDOWN_BASE = 1.5f;
    public override void _Ready()
    {
        _player = GetParent<Player>();
        _magazineCapacity = 4;
        _shots = _magazineCapacity;
        _attackCooldown = ATTACK_COOLDOWN_BASE;
        _reloadTime = 3.0f;

        Main.Hud.SetMaxBullets(4);
    }

    public override void Shoot()
    {
        // If we cannot attack, don't.
        if (!_attackEnabled || _shots <= 0) return;

        // Stop reloading.
        _reloadTimer.Stop();
        
        // Calculate the cooldown time between attacks.
        _attackCooldown = ATTACK_COOLDOWN_BASE - (0.1f * _player.GetItemCount(Item.ItemType.AttackSpeed));

        _shots--;
        FireParticles();
        Main.Hud.SetBullets(_shots);

        int critCount = _player.GetItemCount(Item.ItemType.CritChance);
        bool crit = true;

        if (critCount < 10)
        {
            crit = GD.RandRange(1, 100) <= 10 * critCount;
        }

        for (int i = 0; i < BULLETS_PER_SHOT; i++)
        {
            var random_offset = new Vector2(GD.RandRange(-20, 20), GD.RandRange(-20, 20));

            Bullet bullet = _bulletScene.Instantiate<Bullet>();

            bullet.Damage = 1;
            bullet.Crit = crit;

            bullet.Position = _bulletSpawnLocation.GlobalPosition;
            bullet.Velocity = bullet.Position - _player.Position + random_offset;

            // Kick the player away from the bullet velocity.
            _player.Velocity -= new Vector2(x:bullet.Velocity.X, y:bullet.Velocity.Y);
            // Move the player after each bullet is fired.
            _player.MoveAndSlide();

            _player.GetParent().AddChild(bullet);
        }

        Recoil();

        if (_shots <= 0)
        {
            Reload();
        }
    }

    protected override void Reload()
    {
        if (_shots < _magazineCapacity)
        {
            _reloadTimer.Start();
        }
    }

    private void OnReloadTimerTimeout()
    {
        if (_shots >= _magazineCapacity)
        {
            _shots = _magazineCapacity;
            _reloadTimer.Stop();
        }
        else
        {
            _shots++;
            Main.Hud.SetBullets(_shots);
        }
    }

    private void FireParticles()
    {
        _shotBurst.Emitting = true;
    }
}
