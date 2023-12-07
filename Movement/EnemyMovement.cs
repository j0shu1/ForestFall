using Godot;

public class EnemyMovement : MovementComponent
{
    private const float _jumpVelocity = -500.0f;
    public override void Move(CharacterBody2D body, double delta)
    {
        Enemy enemy = body as Enemy;

        var direction = Vector2.Zero;

        if (!enemy.Attacking && !enemy.Dead)
        {
            direction = enemy.ToLocal(enemy.GetNode<NavigationAgent2D>("NavigationAgent2D").GetNextPathPosition()).Normalized();

            // Update the direction the enemy is facing.
            if (direction.X > 0.25)
                enemy.FacingRight = true;
            if (direction.X < -0.25)
                enemy.FacingRight = false;
        }

        Vector2 velocity = new(direction.X * Speed, 0);

        if (!enemy.IsOnFloor())
            velocity.Y = enemy.Velocity.Y + _gravity * (float)delta;
        else if (direction.Y < -0.5 && !enemy.Attacking)
            velocity.Y = _jumpVelocity;

        enemy.Velocity = velocity;
        enemy.MoveAndSlide();

        HandleEnemyAnimation(enemy);
    }

    private void HandleEnemyAnimation(Enemy enemy)
    {
        var enemySprite = enemy.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        enemySprite.FlipH = !enemy.FacingRight;

        if (enemy.Velocity.Abs().X > 0.1 && !enemy.Attacking && !enemy.Dead)
            enemySprite.Play("Run");
    }
}
