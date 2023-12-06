using Godot;

public partial class EndScreen : CanvasLayer
{
    [Export]
    private PackedScene _gameScene;

    [Export]
    private PackedScene _menuScene;
    public void ShowEndScreen(Player player)
    {
        // Display stats.
        GetNode<Label>("Panel/HBoxContainer/Stats/TimeSurvived").Text = $"Seconds Survived: {Main.Hud.TimeElapsed}";
        GetNode<Label>("Panel/HBoxContainer/Stats/DamageDealt").Text = $"Damage Dealt: {Enemy.TotalDamageTaken}";
        GetNode<Label>("Panel/HBoxContainer/Stats/MostDamageDealt").Text = $"Most Damage Dealt: {Enemy.MaxDamageTaken}";
        GetNode<Label>("Panel/HBoxContainer/Stats/DamageTaken").Text = $"Damage Taken: {player.DamageTaken}";
        GetNode<Label>("Panel/HBoxContainer/Stats/AmountHealed").Text = $"Amount Healed: {player.AmountHealed}";
        GetNode<Label>("Panel/HBoxContainer/Stats/LevelReached").Text = $"Level: {Player.Level}";
        GetNode<Label>("Panel/HBoxContainer/Stats/ExperienceGained").Text = $"Experience Gained: {Player.ExperienceGained}";
        GetNode<Label>("Panel/HBoxContainer/Stats/GoldCollected").Text = $"Gold Collected: {player.GoldCollected}";
        GetNode<Label>("Panel/HBoxContainer/Stats/GoldSpent").Text = $"Gold Spent: {player.GoldSpent}";

        // Display items collected.
        GetNode<Label>("Panel/HBoxContainer/Items/QuickSipsCollected").Text = $"Quick Sips Collected: {player.GetItemCount(Item.ItemType.AttackSpeed)}";
        GetNode<Label>("Panel/HBoxContainer/Items/FocusingGlassesCollected").Text = $"Focusing Glasses Collected: {player.GetItemCount(Item.ItemType.CritChance)}";
        GetNode<Label>("Panel/HBoxContainer/Items/SpeedySneakersCollected").Text = $"Speedy Sneakers Collected: {player.GetItemCount(Item.ItemType.SpeedBoost)}";
        GetNode<Label>("Panel/HBoxContainer/Items/ArmorPlatesCollected").Text = $"Armor Plates Collected: {player.GetItemCount(Item.ItemType.ArmorPlate)}";
    }

    private void OnPlayAgainButtonPressed()
    {
        ResetVariables();
        GetTree().ChangeSceneToPacked(_gameScene);
    }

    private void OnMainMenuButtonPressed()
    {
        ResetVariables();
        GetTree().ChangeSceneToPacked(_menuScene);
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

    private void ResetVariables()
    {
        Enemy.TotalDamageTaken = 0;
        Enemy.MaxDamageTaken = 0;
        Enemy.Level = 1;
        Player.ExperienceGained = 0;
        Player.Level = 1;
    }
}
