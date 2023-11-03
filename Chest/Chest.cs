using Godot;

public partial class Chest : StaticBody2D
{
	[Export]
	private bool _debug = false;

	private Label _prompt;
	private bool _playerInRange = false;
	private bool _opened = false;
	private PanelContainer _container;

    public override void _Ready()
    {
		_container = GetNode<PanelContainer>("PanelContainer");
		_prompt = _container.GetNode<Label>("Label");
    }

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
		if (_opened)
			return;

		if (body is Player)
		{
			_playerInRange = true;
			SetPromptVisibility(true);
		}
	}

	private void OnInteractionAreaBodyExited(Node2D body)
	{
		if (_opened)
			return;

		if (body is Player)
		{
			_playerInRange = false;
            SetPromptVisibility(false);
        }
    }

	private void OpenChest()
	{
		if (!_debug)
			_opened = true;
		// TODO: Play animation of chest opening.
		// TODO: Remove the HUD displaying instructions.
		SetPromptVisibility(false);

		// Spawn a random item.
		GetParent().AddChild(Item.CreateRandomItem(location: GlobalPosition));
	}

	private void SetPromptVisibility(bool visible)
	{
        _container.Visible = visible;

		if (visible)
		{
            _prompt.Text = Input.GetConnectedJoypads().Count > 0 ? "Press X to interact" : "Press E to interact";
        }
		else
		{
			_prompt.Text = "";
		}
	}
}
