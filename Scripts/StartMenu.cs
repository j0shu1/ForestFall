using Godot;

public partial class StartMenu : CanvasLayer
{
	[Export]
	private PackedScene MainScene;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionReleased("ui_select"))
        {
            OnStartButtonPressed();
        }
    }
    private void OnStartButtonPressed()
	{
		GetTree().ChangeSceneToPacked(MainScene);
	}
}
