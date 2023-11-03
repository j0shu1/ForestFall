using Godot;

public partial class HealthComponent : Node
{
    [Export]
    public TextureProgressBar HealthBar;
    [Export]
    private Timer _regenTimer;
    [Export]
    private PackedScene _damageParticleScene;

    private int _health;
    private int _maxHealth = 10;
    private int _regenAmount = 1;
    protected Node2D _parent;

    public override void _Ready()
    {
        _parent = (Node2D)GetParent();
        _health = _maxHealth;

        HealthBar.MaxValue = _maxHealth;
        HealthBar.Value = _health;
    }

    public int GetHealth()
    {
        return _health;
    }

    public bool IsDead()
    {
        return _health <= 0;
    }

    public void SetInitialMaxHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
    }

    public void Hurt(int damage)
    {
        // If the target is a player, apply damage reduction and show that the player was hit.
        if (_parent is Player player)
        {
            int armorPlates = player.GetItemCount(Item.ItemType.ArmorPlate);
            damage -= armorPlates * 2;
            damage = damage < 1 ? 1 : damage;

            // Make the character flash red when they take damage.
            Tween tween = CreateTween();
            var sprite = _parent.GetNode<AnimatedSprite2D>("AnimatedSprite2D");

            tween.TweenProperty(sprite, "modulate", Color.Color8(255, 0, 0), 0.05);
            tween.TweenProperty(sprite, "modulate", Color.Color8(255, 255, 255), 0.05);

            // Also flash the player's health bar.
            tween.TweenProperty(HealthBar, "tint_progress", Color.Color8(255, 0, 0), 0.15);
            tween.TweenProperty(HealthBar, "tint_progress", Color.Color8(0, 150, 0), 0.15);
        }

        //GD.Print($"{_parent.Name} took {damage} damage.");
        _health -= damage;
        _regenTimer.Start();

        SpawnDamageParticle(damage);
        HealthBar.Value = _health;

        if (_health <= 0)
        {
            //GD.Print($"{_parent.Name} died.");
            // TODO: Play death animation, disable movement.
            if (_parent is not Player)
                _parent.QueueFree();
            else
            {
                // Player death animation.
                // Change scene to game over.
                // Show statistics for the run.
            }
        }
    }
    public void SpawnDamageParticle(int number)
    {
        Vector2 positionOffset = new(GD.RandRange(-30, 30), GD.RandRange(-30, 30));

        var damageParticle = _damageParticleScene.Instantiate<DamageParticle>();
        damageParticle.SetValue(number);
        damageParticle.GlobalPosition = _parent.GlobalPosition + positionOffset;
        _parent.GetParent().AddChild(damageParticle);
    }

    private void OnRegenTimerTimeout()
    {
        _health += _regenAmount;

        if (_health >= _maxHealth)
        {
            _health = _maxHealth;
            _regenTimer.Stop();
        }
    }
}

