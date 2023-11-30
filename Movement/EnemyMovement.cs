using Godot;

public class EnemyMovement : MovementComponent
{
    private const float _jumpVelocity = -500.0f;
    public override void Move(CharacterBody2D body, double delta)
    {
        Enemy enemy = body as Enemy;
        var direction = enemy.ToLocal(enemy.NavigationAgent.GetNextPathPosition()).Normalized();
        Vector2 velocity = new(0, 0);

        if (!enemy.IsOnFloor())
            velocity.Y = enemy.Velocity.Y + _gravity * (float)delta;
        else if (direction.Y < -0.5 && !enemy.Attacking)
            velocity.Y = _jumpVelocity;
        
        velocity.X = direction.X * Speed;
        enemy.Velocity = velocity;
        enemy.MoveAndSlide();
    }
}
