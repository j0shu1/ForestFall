using Godot;

public abstract class MovementComponent
{
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    protected static float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public abstract void Move(CharacterBody2D body, double delta);
    public float Speed;
}
