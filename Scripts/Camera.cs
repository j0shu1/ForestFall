using Godot;

public partial class Camera : Camera2D
{
	private Player _player;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_player is not null)
		{ 
			GlobalPosition = _player.GlobalPosition;
		}
		else
		{
			Player player = GetParent().GetNode<Player>("Player");

			if (player is not null)
				_player = player;
		}
	}
}
