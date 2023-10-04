using Godot;

public partial class Bullet : CharacterBody2D
{
	public const float Speed = 300.0f;
	private static StringName _damage = new("Damage"); 
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		// MoveAndCollide returns a KinematicCollision2D object if we want to do something with a collision.
		KinematicCollision2D collision = MoveAndCollide(Velocity.Normalized() * (float)delta * Speed);
		if (collision is not null)
		{
			// This is a CharacterBody2D if the thing hit is an enemy or player,
			// but StaticBody2D if the entity hit is the floor.
			var collisionObject = collision.GetCollider();

            if (collisionObject.GetClass() == "CharacterBody2D")
            {
				collisionObject.Call(_damage, 5);
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
