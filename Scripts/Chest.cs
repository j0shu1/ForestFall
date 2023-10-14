using Godot;

public partial class Chest : StaticBody2D
{
	private bool _playerInRange = false;
	private bool _opened = false;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEvent input)
		{
			if (input.IsActionPressed("interact") && _playerInRange && !_opened)
			{
				OpenChest();
			}
		}
    }

    private void OnInteractionAreaBodyEntered(Node2D body)
	{
		if (body is Player)
		{
			_playerInRange = true;
			// TODO: Tell the HUD to show how to interact.
		}
	}

	private void OnInteractionAreaBodyExited(Node2D body)
	{
		if (body is Player)
		{
			_playerInRange = false;
			// TODO: Tell the HUD to remove how to interact.
		}
	}

	private void OpenChest()
	{
		_opened = true;
		// TODO: Play animation of chest opening.
		// TODO: Remove the HUD displaying instructions.

		// Spawn a random item.
		GetParent().AddChild(ItemFactory.CreateRandomItem(location: GlobalPosition));
	}
}
