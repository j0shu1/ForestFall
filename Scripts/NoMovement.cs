using Godot;

public class NoMovement : MovementComponent
{
    public override void Move(CharacterBody2D body, double delta)
    {
        // Only apply gravity to the body.
        Vector2 velocity = body.Velocity;

        if (!body.IsOnFloor())
            velocity.Y += _gravity * (float)delta;

        body.Velocity = velocity;
    }
}