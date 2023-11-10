using Godot;

public class EnemyMovement : MovementComponent
{
    public override void Move(CharacterBody2D body, double delta)
    {
        Enemy enemy = body as Enemy;
        var direction = enemy.ToLocal(enemy.NavigationAgent.GetNextPathPosition()).Normalized();
        direction *= Speed;
        enemy.Velocity = new Vector2(direction.X, enemy.Velocity.Y + _gravity * (float)delta);
        enemy.MoveAndSlide();
    }
}
