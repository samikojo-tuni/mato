using Godot;
using System;

namespace SnakeGame
{
	public partial class Settings : Node
	{
		[Signal] public delegate void LanguageChangedEventHandler(string langCode);

		private SettingsData _data = null;

		public override void _Ready()
		{
			base._Ready();

			// Lataa asetukset tiedostosta.
			_data = LoadSettings();
			ApplyData(_data);
		}

		private void ApplyData(SettingsData data)
		{
			if (data == null)
			{
				return;
			}

			// Aseta äänenvoimakkuudet.
			SetVolume("Master", data.MasterVolume);
			SetVolume("Music", data.MusicVolume);
			SetVolume("SFX", data.SfxVolume);

			// Aseta kieli.
			SetLanguage(data.LangCode);
		}

		public bool SaveSettings()
		{
			if (_data == null)
			{
				return false;
			}

			ConfigFile settingsConfig = new ConfigFile();
			settingsConfig.SetValue("Localization", "LangCode", _data.LangCode);
			settingsConfig.SetValue("Audio", "MasterVolume", _data.MasterVolume);
			settingsConfig.SetValue("Audio", "MusicVolume", _data.MusicVolume);
			settingsConfig.SetValue("Audio", "SfxVolume", _data.SfxVolume);

			if (settingsConfig.Save(Config.SettingsFile) != Error.Ok)
			{
				GD.PrintErr("Failed to save settings.");
				return false;
			}

			return true;
		}

		private SettingsData LoadSettings()
		{
			SettingsData data = null;
			ConfigFile settingsConfig = new ConfigFile();
			if (settingsConfig.Load(Config.SettingsFile) == Error.Ok)
			{
				// Settings-tiedosto ladattiin onnistuneesti.
				data = new SettingsData
				{
					LangCode = (string)settingsConfig.GetValue("Localization", "LangCode", "en"),
					MasterVolume = (float)settingsConfig.GetValue("Audio", "MasterVolume", -6.0f),
					MusicVolume = (float)settingsConfig.GetValue("Audio", "MusicVolume", -6.0f),
					SfxVolume = (float)settingsConfig.GetValue("Audio", "SfxVolume", -6.0f)
				};
			}
			else
			{
				// Asetustiedostoa ei löydetty, luodaan oletusasetukset.
				data = SettingsData.CreateDefaults();
			}

			return data;
		}

		public bool SetVolume(string busName, float volumeDB)
		{
			if (_data == null)
			{
				return false;
			}

			int busIndex = AudioServer.GetBusIndex(busName);
			if (busIndex < 0)
			{
				GD.PrintErr($"Bus '{busName}' not found.");
				return false;
			}

			AudioServer.SetBusVolumeDb(busIndex, volumeDB);

			switch (busName)
			{
				case "Master":
					_data.MasterVolume = volumeDB;
					break;
				case "Music":
					_data.MusicVolume = volumeDB;
					break;
				case "SFX":
					_data.SfxVolume = volumeDB;
					break;
				default:
					GD.PrintErr($"Unknown bus '{busName}'.");
					break;
			}

			return true;
		}

		public bool GetVolume(string busName, out float volumeDB)
		{
			int busIndex = AudioServer.GetBusIndex(busName);
			if (busIndex < 0)
			{
				GD.PrintErr($"Bus '{busName}' not found.");
				volumeDB = float.NaN;
				return false;
			}

			volumeDB = AudioServer.GetBusVolumeDb(busIndex);
			return true;
		}

		public string GetLanguage()
		{
			return TranslationServer.GetLocale();
		}

		public bool SetLanguage(string langCode)
		{
			if (_data == null)
			{
				return false;
			}

			_data.LangCode = langCode;
			TranslationServer.SetLocale(langCode);

			// Välitä tieto kielen vaihtumisesta.
			EmitSignal(SignalName.LanguageChanged, langCode);

			_data.LangCode = langCode;

			return true;
		}
	}
}