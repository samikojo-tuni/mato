using Godot;
using System;

namespace SnakeGame.UI
{
	public partial class BottomUIControl : Control
	{
		[Export] private BaseButton _settingsButton = null;

		public override void _Ready()
		{
			base._Ready();

			_settingsButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnSettingsButtonPressed)));
		}

		private void OnSettingsButtonPressed()
		{
			Level.Current.SettingsWindow.Open();
		}
	}
}