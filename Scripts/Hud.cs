using Godot;
using System.Collections;

public partial class Hud : CanvasLayer
{
	[Export]
	private TextureProgressBar _healthBar;
	[Export]
	private Sprite2D _magazineBackground;
	[Export]
	private Sprite2D _magazineForeground;
	[Export]
	private PanelContainer _itemPanelContainer;
	[Export]
	private Timer _displayTimer;
	[Export]
	private TextureRect _itemImage;
	[Export]
	private Label _itemNameLabel;
	[Export]
	private Label _itemDescriptionLabel;

	private Queue _itemCollectionQueue = new();

	public void DisplayCollectedItem(Item.ItemType itemType)
	{
		_itemCollectionQueue.Enqueue(itemType);

		if (_displayTimer.IsStopped())
		{
			DisplayNextItem();
			_itemPanelContainer.Visible = true;
			_displayTimer.Start();
		}
	}

	public void SetMaxBullets(int amount)
	{
		Rect2 region = new(0, 0, 10 * amount, 12);
		_magazineBackground.RegionRect = region;
		_magazineForeground.RegionRect = region;
	}
	public void SetBullets(int amount)
	{
		_magazineForeground.RegionRect = new(0, 0, 10 * amount, 12);
	}



	private void OnDisplayTimerTimeout()
	{
		// If the queue is empty, stop the timer and stop displaying the item panel container.
		if (_itemCollectionQueue.Count == 0)
		{
			_displayTimer.Stop();
			_itemPanelContainer.Visible = false;
			return;
		}

		DisplayNextItem();
	}

	private void DisplayNextItem()
	{
		// Display the next item in the queue.
		Item.ItemType itemType = (Item.ItemType)_itemCollectionQueue.Dequeue();
		_itemImage.Texture = Item.GetTexture2D(itemType);
		_itemNameLabel.Text = Item.GetName(itemType);
		_itemDescriptionLabel.Text = Item.GetDescription(itemType);
	}


}
