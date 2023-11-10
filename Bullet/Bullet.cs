using Godot;

public partial class Bullet : CharacterBody2D
{
	public bool Crit;
	public bool Piercing = false;
	public int Damage;
	public const float Speed = 2000.0f;

	[Export]
	private Sprite2D _sprite;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
		if (Crit)
		{
			Damage *= 2;
			_sprite.Modulate = Color.Color8(255, 0, 0);
		}
    }

    public override void _PhysicsProcess(double delta)
	{
		// MoveAndCollide returns a KinematicCollision2D object if we want to do something with a collision.
		KinematicCollision2D collision = MoveAndCollide(Velocity.Normalized() * (float)delta * Speed);

		if (collision is not null)
		{
			if (collision.GetCollider() is Enemy enemy)
			{
				enemy.Hurt(Damage);

				if (!Piercing)
					QueueFree();
			}
			else
			{
				QueueFree();
			}

		}

	}

	private void OnDeleteTimerTimeout()
	{
		QueueFree();
	}
}
