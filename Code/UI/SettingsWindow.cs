using Godot;
using System;

namespace SnakeGame.UI
{
	public partial class SettingsWindow : DialogWindow
	{
		[Export] private BaseButton _fiButton = null;
		[Export] private BaseButton _enButton = null;
		[Export] private AudioControl _masterAudioControl = null;
		[Export] private AudioControl _musicAudioControl = null;
		[Export] private AudioControl _effectsAudioControl = null;
		private string _originalLanguage = null;

		public override void _Ready()
		{
			base._Ready();

			if (_fiButton != null)
			{
				_fiButton.Connect(Button.SignalName.Pressed,
					new Callable(this, nameof(OnFiButtonPressed)));
			}

			if (_enButton != null)
			{
				_enButton.Connect(Button.SignalName.Pressed,
					new Callable(this, nameof(OnEnButtonPressed)));
			}
		}

		public void Initialize()
		{
			if (_masterAudioControl != null)
			{
				_masterAudioControl.Initialize();
			}

			if (_musicAudioControl != null)
			{
				_musicAudioControl.Initialize();
			}

			if (_effectsAudioControl != null)
			{
				_effectsAudioControl.Initialize();
			}

			_originalLanguage = Level.Current.Settings.GetLanguage();
		}

		private void OnFiButtonPressed()
		{
			GD.Print("FI button pressed");
			Level.Current.Settings.SetLanguage("fi");
		}

		private void OnEnButtonPressed()
		{
			GD.Print("EN button pressed");
			Level.Current.Settings.SetLanguage("en");
		}

		public override void Open()
		{
			base.Open();
			Level.Current.Pause();
		}

		public override void Close()
		{
			Level.Current.Resume();
			base.Close();
		}

		protected override void OnOkButtonPressed()
		{
			Level.Current.Settings.SaveSettings();

			// Suorita lopuksi DialogWindow:n oma toiminnallisuuus.
			base.OnOkButtonPressed();
		}

		protected override void OnCancelButtonPressed()
		{
			RestoreValues();

			// Suorita lopuksi DialogWindow:n oma toiminnallisuuus.
			base.OnCancelButtonPressed();
		}

		private void RestoreValues()
		{
			// Palauta kieli alkuperäiseksi.
			Level.Current.Settings.SetLanguage(_originalLanguage);
			_masterAudioControl.RestoreValues();
			_musicAudioControl.RestoreValues();
			_effectsAudioControl.RestoreValues();
		}
	}
}