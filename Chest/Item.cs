using Godot;
using System;
using System.Collections.Generic;

public partial class Item : Sprite2D
{
    public ItemType Type;

    /* Steps to add an item:
     * 1. Add the ItemType to the enum.
     * 2. Add name, description, and Sprite2D to the _itemData dictionary.
     * 3. Change the code where applicable to get the number of items
     *      of that type from the player and do something with it.
     */

    public enum ItemType
    {
        CritChance,
        ArmorPlate,
        SpeedBoost,
        AttackSpeed
    }

    private static readonly PackedScene _itemScene = (PackedScene)ResourceLoader.Load("res://Chest/item.tscn");

    private static readonly Dictionary<ItemType, (string Name, string Description, Texture2D Texture)> _itemData = new()
    {
     /*
      * { ItemType.Name,      
      *     ("Name",
      *     "Description",
      *     (Texture2D)ResourceLoader.Load("SpritePath"))}
     */
        { ItemType.CritChance,
            ("Focusing Glasses",
            "Chance to critically strike, dealing double damage.",
            (Texture2D)ResourceLoader.Load("res://Assets/crit_chance.png"))},
        
        { ItemType.ArmorPlate,
            ("Armor Plate",
            "Reduces damage taken by a flat amount.",
            (Texture2D)ResourceLoader.Load("res://Assets/armor_plate.png"))},

        { ItemType.SpeedBoost,
            ("Speedy Sneakers",
            "Increases movement speed.",
            (Texture2D)ResourceLoader.Load("res://Assets/speed_boost.png"))},

        { ItemType.AttackSpeed,
            ("Quick Sips",
            "Decreases time between attacks.",
            (Texture2D)ResourceLoader.Load("res://Assets/attack_speed.png"))}
    };

    public async override void _Ready()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(this, "global_position", GlobalPosition + new Vector2(0, -65), 0.5);

		var collisionDetection = GetNode<Area2D>("CollectionArea").GetNode<CollisionShape2D>("CollisionShape2D");
		
		// Enable the hitbox after waiting for a period of time.
        await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);
		collisionDetection.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
    }

    public static Item CreateRandomItem(Vector2 location)
    {
        Item item = _itemScene.Instantiate<Item>();

        ItemType itemType = GetRandomItemType();

        item.Type = itemType;
        item.Texture = GetTexture2D(itemType);
        item.GlobalPosition = new Vector2(x: location.X, y: location.Y); // Minus indicates to hover above, not below.

        return item;
    }

    public static string GetName(ItemType itemType)
    {
        return _itemData[itemType].Name;
    }

    public static string GetDescription(ItemType itemType)
    {
        return _itemData[itemType].Description;
    }

    public static Texture2D GetTexture2D(ItemType itemType)
    {
        return _itemData[itemType].Texture;
    }

    private static ItemType GetRandomItemType()
    {
        ItemType[] itemTypes = Enum.GetValues<ItemType>();
        return itemTypes[GD.RandRange(0, itemTypes.Length - 1)];
    }

    private void OnCollectionAreaBodyEntered(Node2D body)
	{
        if (body is Player player)
        {
			player.AddItem(Type);
            Visible = false;
            GetNode<CollisionShape2D>("CollectionArea/CollisionShape2D").Disabled = true;
            GetNode<AudioStreamPlayer2D>("PickupSound").Play();
			GetNode<Timer>("DespawnTimer").Start();
        }
	}

    private void OnDespawnTimerTimeout()
    {
        QueueFree();
    }

    private void OnStartParticlesTimeout()
    {
        GetNode<CpuParticles2D>("CPUParticles2D").Emitting = true;
    }
}
