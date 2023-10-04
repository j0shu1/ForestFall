using Godot;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _UnhandledKeyInput(InputEvent @event)
    {
		if (@event is InputEventKey inputEventKey)
			if (inputEventKey.IsActionPressed("ui_cancel"))
				GetTree().Quit();
    }
	public int Test()
	{
		return 1;
	}
}
