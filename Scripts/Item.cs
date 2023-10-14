using Godot;

public partial class Item : Sprite2D
{
	public string ItemName;
	private static StringName _addItem = new("AddItem");
	private Variant[] _args;
    private Player _player;

    public async override void _Ready()
    {
		_args = new Variant[] { ItemName };
		
		var collisionDetection = GetNode<Area2D>("CollectionArea").GetNode<CollisionShape2D>("CollisionShape2D");
		
		// Enable the hitbox after waiting for one second.
        await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);
		collisionDetection.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
    }

    private void OnCollectionAreaBodyEntered(Node2D body)
	{
        if (body is Player player)
        {
			player.Call(method:_addItem, args:_args);
			// TODO: Create pickup particles at this location.
			QueueFree();
        }
	}
}
