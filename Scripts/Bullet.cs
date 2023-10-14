using Godot;

public partial class Bullet : CharacterBody2D
{
	public int Damage;
	public const float Speed = 2000.0f;
	private static StringName _damage = new("Damage"); 
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double delta)
	{
		// MoveAndCollide returns a KinematicCollision2D object if we want to do something with a collision.
		KinematicCollision2D collision = MoveAndCollide(Velocity.Normalized() * (float)delta * Speed);
		if (collision is not null)
		{
			if (collision.GetCollider() is Node node && node.IsInGroup("Enemy"))
			{
				node.Call(_damage, Damage);
			}

            QueueFree();
		}
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		// Delete the bullet when it leaves the screen.
		QueueFree();
	}
}
