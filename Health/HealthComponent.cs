using Godot;

public partial class HealthComponent : Node
{
    [Export]
    public TextureProgressBar HealthBar;
    [Export]
    private Timer _regenTimer;
    [Export]
    private PackedScene _damageParticleScene;

    private int _health = 0;
    private int _maxHealth = 0;
    private int _regenAmount = 1;
    protected Node2D _parent;

    private float _healthIncreaseMultiplier = 0;
    private int _linearHealthIncrease = 10;
    public override void _Ready()
    {
        _parent = (Node2D)GetParent();
        _health = _maxHealth;

        UpdateHealthBar();
    }

    public int GetHealth()
    {
        return _health;
    }

    public void LevelUp()
    {
        int healthIncrease = _linearHealthIncrease + (int)(_maxHealth * _healthIncreaseMultiplier);
        _maxHealth += healthIncrease;
        _health += healthIncrease;
        
        UpdateHealthBar();
    }

    public bool IsDead()
    {
        return _health <= 0;
    }

    public void SetInitialMaxHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
        _health = maxHealth;
        UpdateHealthBar();
    }

    public void SetHealthIncreaseMultiplier(float multiplier)
    {
        _healthIncreaseMultiplier = multiplier;
    }

    public void SetLinearHealthIncrease(int increase)
    {
        _linearHealthIncrease = increase;
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
        UpdateHealthBar();
        _regenTimer.Start();

        SpawnDamageParticle(damage);

        if (_health <= 0)
        {
            //GD.Print($"{_parent.Name} died.");
            if (_parent is Enemy enemy)
            {
                enemy.Die();
            }
            else if (_parent is Player p)
            {
                p.Die();
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

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        HealthBar.MaxValue = _maxHealth;
        HealthBar.Value = _health;
    }
}

