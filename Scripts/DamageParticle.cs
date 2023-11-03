using Godot;
using System;

public partial class DamageParticle : Node2D
{
	private Label _damageLabel;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition += Vector2.Down * (float)delta;
	}

    public override void _Ready()
    {
		_damageLabel ??= GetNode<Label>("Label");
		Tween tween = CreateTween();
		tween.SetParallel(true);

		//up and to the right.
		tween.TweenProperty(_damageLabel, "global_position", _damageLabel.GlobalPosition - new Vector2(0, 50), 2);
		//disappear
		tween.TweenProperty(_damageLabel, "theme_override_colors/font_color", Color.Color8(255, 0, 0, 0), 2);
        tween.TweenProperty(_damageLabel, "theme_override_colors/font_shadow_color", Color.Color8(0, 0, 0, 0), 2);
    }

    public void SetValue(double value)
	{
        string text = value switch
        {
			// expression => result
            >= 1000000 => $"{Math.Round(value / 1000000, 1)}M",
            >= 1000 => $"{Math.Round(value / 1000, 2)}K",
            _ => Math.Round(value).ToString(),
        };

        _damageLabel ??= GetNode<Label>("Label");

		int overrideValue = (int)Mathf.Log(value) * 5 + 40;
        _damageLabel.AddThemeFontSizeOverride("font_size", overrideValue);
        _damageLabel.Text = text;
	}
	private void OnDespawnTimerTimeout()
	{
		QueueFree();
	}
}
