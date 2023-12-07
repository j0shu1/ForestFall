using Godot;

public partial class Chest : StaticBody2D
{
	[Export]
	private bool _debug = false;
	[Export]
	private PanelContainer _panelContainer;
	[Export]
	private Label _prompt;

	private Player _playerInRange;
	private bool _opened = false;
	private static int _cost; // This must be set externally through SetChestCost.

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEvent input)
		{
			if (input.IsActionPressed("interact") && _playerInRange is not null && !_opened)
			{
				if (_playerInRange.Purchase(_cost))
					OpenChest();
			}
		}
    }

	public static void SetChestCost(int cost)
	{
		_cost = cost;
	}

    private void OnInteractionAreaBodyEntered(Node2D body)
	{
		if (_opened)
			return;

		if (body is Player player)
		{
			_playerInRange = player;
			SetPromptVisibility(true);
		}
	}

	private void OnInteractionAreaBodyExited(Node2D body)
	{
		if (_opened)
			return;

		if (body is Player)
		{
			_playerInRange = null;
            SetPromptVisibility(false);
        }
    }

	private void OpenChest()
	{
		if (!_debug)
			_opened = true;
		GetNode<AudioStreamPlayer2D>("PurchaseSound").Play();
		SetPromptVisibility(false);

		// Play animation of chest opening.
		var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("Open");
		sprite.AnimationLooped += () => {
			// Stop looping the animation
			sprite.Play("Opened");
			sprite.Stop();

			GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;

			// Spawn a random item.
			GetParent().AddChild(Item.CreateRandomItem(location: GlobalPosition));
		};
	}

	private void SetPromptVisibility(bool visible)
	{
        _panelContainer.Visible = visible;

		_prompt.Text = visible ? 
			Input.GetConnectedJoypads().Count > 0 ? 
				$"Press X to interact (Cost: {_cost})" :
				$"Press E to interact (Cost: {_cost})"
			: "";
	}
}
