using Godot;
using System;

namespace SnakeGame.UI
{
	public partial class AudioControl : Control
	{
		[Export] private Slider _volumeSlider = null;
		[Export] private string _busName = "Master";
		private float _originalVolume = 0.0f;

		public override void _Ready()
		{
			base._Ready();

			_volumeSlider.Connect(Slider.SignalName.ValueChanged,
				new Callable(this, nameof(OnVolumeSliderValueChanged)));
		}

		public void Initialize()
		{
			// Äänenvoimakkuuden alustus.
			if (!Level.Current.Settings.GetVolume(_busName, out _originalVolume))
			{
				GD.PrintErr("Äänenvoimakkuuden alustus epäonnistui.");
			}

			_volumeSlider.Value = Mathf.DbToLinear(_originalVolume);
		}

		public void RestoreValues()
		{
			float linearVolume = Mathf.DbToLinear(_originalVolume);
			// Tee palautus sliderin kautta. Tämä asettaa sliderin
			// oikeaan arvoon asetusten seuraavaa avausta varten ja
			// kutsuu myös eventtiä, joka päivittää äänenvoimakkuuden.
			_volumeSlider.Value = linearVolume;
		}

		private void OnVolumeSliderValueChanged(float value)
		{
			UpdateVolume();
		}

		private void UpdateVolume()
		{
			// Äänenvoimakkuus lineaarisella asteikolla [0,1].
			float linearVolume = (float)_volumeSlider.Value;

			// Äänenvoimakkuus desibeliasteikolla [-80,0].
			float decibelVolume = Mathf.LinearToDb(linearVolume);

			// Asetetaan äänenvoimakkuus ääni-bussille.
			Level.Current.Settings.SetVolume(_busName, decibelVolume);
		}
	}
}
