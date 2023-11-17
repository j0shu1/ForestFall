using Godot;
using System.Collections;

public partial class Hud : CanvasLayer
{
	// Health bar components.
	[Export]
	private TextureProgressBar _healthBar;
	[Export]
	private Label _healthLabel;

	// Ammunition UI components.
	[Export]
	private Sprite2D _magazineBackground;
	[Export]
	private Sprite2D _magazineForeground;

	// Item collection components.
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

	// Enemy level components.
	[Export]
	private TextureProgressBar _enemyLevelBar;
	[Export]
	private Label _enemyLevelLabel;

	// Player level components.
	[Export]
	private TextureProgressBar _expBar;
	[Export]
	private Label _currentLevelLabel;
	[Export]
	private Label _nextLevelLabel;

	[Export]
	private Label _moneyLabel;

	private Queue _itemCollectionQueue = new();

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

	public void AddExp(int amount)
	{
		_expBar.Value += amount;

		while (_expBar.Value > _expBar.MaxValue)
		{
			_expBar.Value -= _expBar.MaxValue;
			Player.Level++;
			GetTree().CallGroup("Player", "LevelUp");
			_currentLevelLabel.Text = Player.Level.ToString();
			_nextLevelLabel.Text = (Player.Level + 1).ToString();
		}
	}

	public void UpdateMoney(int amount)
	{
		_moneyLabel.Text = amount.ToString();
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

	private void DisplayNextItem()
	{
		// Display the next item in the queue.
		Item.ItemType itemType = (Item.ItemType)_itemCollectionQueue.Dequeue();
		_itemImage.Texture = Item.GetTexture2D(itemType);
		_itemNameLabel.Text = Item.GetName(itemType);
		_itemDescriptionLabel.Text = Item.GetDescription(itemType);
	}

	private void OnEnemyLevelTimerTimeout()
	{
		_enemyLevelBar.Value++;

		if (_enemyLevelBar.Value > _enemyLevelBar.MaxValue)
			LevelUpEnemies();
	}

	private void LevelUpEnemies()
	{
		Enemy.Level++;
		GetTree().CallGroup("Enemy", "LevelUp");
		_enemyLevelLabel.Text = $"Enemy Level: {Enemy.Level}";
		_enemyLevelBar.Value = 0;
	}

	private void OnHealthBarValueChanged(float value)
	{
		_healthLabel.Text = $"{_healthBar.Value}/{_healthBar.MaxValue}";
	}

	private void OnHealthBarChanged()
	{
		_healthLabel.Text = $"{_healthBar.Value}/{_healthBar.MaxValue}";
    }
}
