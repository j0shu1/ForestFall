using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ItemFactory
{
    private static readonly float HOVER_HEIGHT = 65;
    private static readonly Random _random = new();
    private static readonly PackedScene _itemScene = (PackedScene)ResourceLoader.Load("res://Scenes/item.tscn");
    private static readonly Dictionary<string, string> _itemTypes = new()
    {
     // { "Name",   "SpritePath"     }
        { "Damage", "res://Assets/placeholder weapon.png" },
        { "CritChance", "res://Assets/placeholder weapon.png" },
        { "ArmorPlate", "res://Assets/placeholder weapon.png" },
        { "SpeedBoost", "res://Assets/placeholder weapon.png" }
    };
    public static Item CreateRandomItem(Vector2 location)
    {
        Item item = _itemScene.Instantiate<Item>();

        var itemType = _itemTypes.ElementAt(_random.Next(_itemTypes.Count));

        string itemName = itemType.Key;
        string spritePath = itemType.Value;

        item.ItemName = itemName;
        item.Texture = (Texture2D)ResourceLoader.Load(spritePath);
        item.GlobalPosition = new Vector2(x: location.X, y: location.Y - HOVER_HEIGHT); // Minus indicates to hover above, not below.

        return item;
    }
}