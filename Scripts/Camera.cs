using Godot;

public partial class Camera : Camera2D
{
	private Player _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetParent().GetNode<Player>("Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = _player.GlobalPosition;

		//If the player is on either edge of the screen, cause the camera to follow them.
		//if (_player.GlobalPosition > this.)
		//{

		//}
	}
}
