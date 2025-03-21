using System;
using Godot;

namespace SnakeGame.UI
{
	public partial class TopUIControl : Node
	{
		[Export] private Label _scoreLabel = null;
		[Export] private BaseButton _restartButton = null;

		public override void _Ready()
		{
			// Ala kuuntelemaan napin Pressed eventiä. Event laukeaa, kun
			// nappia painetaan.
			if (_restartButton != null)
			{
				// Väitetään suoritettava metodi eventille. HUOM! Metodia
				// ei suoriteta tässä kohdassa. Ainoastaan viittaus metodiin välitetään
				// eventille.
				_restartButton.Pressed += OnRestartPressed;
			}
		}

		public void Initialize()
		{
			// Kuuntele kielen vaihtumista.
			Level.Current.Settings.Connect(Settings.SignalName.LanguageChanged,
				new Callable(this, nameof(OnLanguageChanged)));
		}

		private void OnLanguageChanged(string langCode)
		{
			// Score-tekstin päivitys, kun kieli vaihtuu.
			SetScore(Level.Current.Score);
		}

		private void OnRestartPressed()
		{
			Level.Current.ResetGame();
		}

		public void SetScore(int score)
		{
			_scoreLabel.Text = Tr("SCORE") + ": " + score.ToString();
		}
	}
}