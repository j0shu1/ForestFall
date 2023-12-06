using Godot;

public class PlayerMovement : MovementComponent
{
    public const float JumpVelocity = -400.0f;
    public override void Move(CharacterBody2D body, double delta)
    {
        Player player = (Player)body;
        Vector2 velChange = player.Velocity;
        bool isOnFloor = player.IsOnFloor();

        // Add the gravity.
        if (!isOnFloor)
        {
            velChange.Y += _gravity * (float)delta;
        }

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
            if (direction.X > 0.5f)
                direction.X = 1;
            else if (direction.X < -0.5f)
                direction.X = -1;

            velChange.X = direction.X * Speed * (1 + 0.10f * player.GetItemCount(Item.ItemType.SpeedBoost));
            player.FacingRight = velChange.X > 0;
        }
        else
        {
            velChange.X = Mathf.MoveToward(player.Velocity.X, 0, Speed);
        }

        player.Velocity = velChange;
        HandlePlayerAnimation(player, isOnFloor);
        player.MoveAndSlide();
    }

    private void HandlePlayerAnimation(Player player, bool isOnFloor)
    {
        var playerSprite = player.GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (!isOnFloor)
        {
            playerSprite.Play("Fall");
        }
        else
        {
            switch (player.Velocity.Abs().X)
            {
                case > 0.5f:
                    playerSprite.Play("Run");
                    break;
                default:
                    playerSprite.Play("Idle");
                    break;
            }
        }



        playerSprite.FlipH = !player.FacingRight;
    }
}