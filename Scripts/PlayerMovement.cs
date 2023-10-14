using Godot;

public class PlayerMovement : MovementComponent
{
    public const float Speed = 400.0f;
    public const float JumpVelocity = -400.0f;
    public override void Move(CharacterBody2D body, double delta)
    {
        Player player = (Player)body;
        Vector2 velChange = player.Velocity;
        bool isOnFloor = player.IsOnFloor();

        // Add the gravity.
        if (!isOnFloor)
            velChange.Y += _gravity * (float)delta;

        // Handle Jump.
        if (isOnFloor &&
            (Input.IsActionJustPressed("ui_accept")
            || Input.IsActionJustPressed("jump")))
        {
            velChange.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        direction += Input.GetVector("move_left", "move_right", "jump", "move_down");
        direction = direction.Normalized();
        if (direction != Vector2.Zero)
        {
            velChange.X = direction.X * Speed * (1 + 0.10f * player.ItemCount("SpeedBoost"));
            player.facingRight = velChange.X > 0;
        }
        else
        {
            velChange.X = Mathf.MoveToward(player.Velocity.X, 0, Speed);
        }

        player.Velocity = velChange;
    }
}