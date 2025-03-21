using System;
using System.IO;
using Godot;
using Godot.Collections;
using SnakeGame.UI;

namespace SnakeGame
{
	public partial class Level : Node2D
	{
		public enum EffectType
		{
			None = 0,
			Collect,
			Death,
		}

		// Static on luokan ominaisuus, ei siitä luotujen olioiden.
		// Kaikki luokasta luodut olio jakavat saman staattisen muuttujan.
		private static Level _current = null;
		public static Level Current
		{
			get { return _current; }
		}

		[Export] private string _snakeScenePath = "res://Character/Snake.tscn";
		[Export] private string _appleScenePath = "res://Levels/Collectables/Apple.tscn";
		[Export] private string _nuclearWasteScenePath = "res://Levels/Collectables/NuclearWaste.tscn";
		[Export] private TopUIControl _topUIControl = null;
		[Export] private Vector2I _startPosition = new Vector2I();
		[Export] private InputDialog _nameInput = null;
		[Export] private PackedScene _collectEffectScene = null;
		[Export] private AudioStreamPlayer2D _deathSound = null;
		[Export] private AudioStreamPlayer2D _collectSound = null;

		// Sceneviittaukset, joista voidaan luoda olioita.
		private PackedScene _snakeScene = null;
		private PackedScene _appleScene = null;
		private PackedScene _nuclearWasteScene = null;

		private SceneTree _levelScene = null;

		private int _score = 0;

		private Grid _grid = null;

		// Varsinaiset viittaukset Level-skenessä oleviin olioihin.
		private Snake _snake = null;
		// Omenoita voi olla olemassa yksi kerrallaan.
		private Apple _apple = null;
		private NuclearWaste _nuclearWaste = null;
		// Optimointisyystä vain yksi instanssi efektistä, jota kierrätetään aina, kun
		// efektiä tarvitaan.
		private GpuParticles2D _collectEffect = null;

		public int Score
		{
			get { return _score; }
			set
			{
				if (value < 0)
				{
					_score = 0;
				}
				else
				{
					_score = value;
				}

				if (_topUIControl != null)
				{
					_topUIControl.SetScore(_score);
				}
			}
		}

		public Grid Grid
		{
			get { return _grid; }
		}

		public Snake Snake
		{
			get { return _snake; }
		}

		public Settings Settings
		{
			get;
			private set;
		}

		public SettingsWindow SettingsWindow
		{
			get;
			private set;
		}

		// Rakentaja. Käytetään alustamaan olio.
		public Level()
		{
			// _current muuttujaan asetetaan viittaus juuri juotuun Level-olioon.
			// Tälläön Current-propertyn kautta muut oliot pääsevät käsiksi Level-olioon.
			_current = this;
		}

		public override void _Ready()
		{
			_grid = GetNode<Grid>("Grid");
			if (_grid == null)
			{
				GD.PrintErr("Gridiä ei löytynyt Levelin lapsinodeista!");
			}

			if (_nameInput != null)
			{
				_nameInput.Close();
				_nameInput.Connect(InputDialog.SignalName.DialogClosed, new Callable(this, nameof(OnDialogClosed)));
			}

			Settings = GetNode<Settings>("Settings");
			if (Settings == null)
			{
				GD.PrintErr("Settingsiä ei löytynyt Levelin lapsinodeista!");
			}

			SettingsWindow = GetNode<SettingsWindow>("UI/SettingsWindow");
			if (SettingsWindow != null)
			{
				SettingsWindow.Initialize();
				SettingsWindow.Close();
			}

			TopUIControl topUIControl = GetNode<TopUIControl>("UI/TopUi");
			if (topUIControl != null)
			{
				topUIControl.Initialize();
			}

			ResetGame();
		}

		public override void _Process(double delta)
		{
			if (Input.IsActionJustPressed("QuickSave"))
			{
				Save();
			}
			if (Input.IsActionJustPressed("QuickLoad"))
			{
				if (Load())
				{
					GD.Print("Peli ladattu!");
					_nameInput.Close();

					_snake.Start();
				}
			}
		}

		private void OnDialogClosed(bool ok)
		{
			if (ok)
			{
				HighScore highScore = new HighScore();
				highScore.AddScore(_nameInput.GetInput(), Score);
			}
		}

		private Snake CreateSnake()
		{
			if (_snakeScene == null)
			{
				_snakeScene = ResourceLoader.Load<PackedScene>(_snakeScenePath);
				if (_snakeScene == null)
				{
					GD.PrintErr("Madon sceneä ei löydy!");
					return null;
				}
			}

			return _snakeScene.Instantiate<Snake>();
		}

		#region Public methods

		public void PlayAudioEffect(EffectType effectType)
		{
			switch (effectType)
			{
				case EffectType.Collect:
					if (_collectSound != null)
					{
						_collectSound.Play();
					}
					break;
				case EffectType.Death:
					if (_deathSound != null)
					{
						_deathSound.Play();
					}
					break;
				default:
					break;
			}
		}

		public bool ShowCollectEffect(Vector2 position)
		{
			if (_collectEffect == null && _collectEffectScene != null)
			{
				_collectEffect = _collectEffectScene.Instantiate<GpuParticles2D>();
				AddChild(_collectEffect);
			}
			else if (_collectEffect == null && _collectEffectScene == null)
			{
				GD.PrintErr("Collect effect scene not loaded!");
				return false;
			}

			// Aseta efektin sijainti ja käynnistä se. Varmista, että efekti toistetaan vain kerran.
			_collectEffect.Position = position;
			_collectEffect.Restart();
			_collectEffect.OneShot = true;

			return true;
		}

		/// <summary>
		/// Aloittaa uuden pelin.
		/// </summary>
		public void ResetGame()
		{
			// Tuhoa edellinen mato, jos se on olemassa.
			DestroySnake();

			// Luo mato
			_snake = CreateSnake();
			AddChild(_snake);
			_snake.CreateSnake(_startPosition);

			// Nollaa pisteet
			Score = 0;

			// Luo omena
			ReplaceApple();

			// Replace Nuclear Waste.
			ReplaceNuclearWaste();

			_snake.Start();
		}

		public void GameOver()
		{
			// Avaa name dialogi.
			if (_nameInput != null)
			{
				_nameInput.Open();
			}
		}

		public void DestroySnake()
		{
			if (_snake != null)
			{
				// Vapauta madon solut gridillä.
				_snake.ReleaseCells();

				_snake.QueueFree();
				_snake = null;
			}
		}

		public void ReplaceNuclearWaste()
		{
			DestroyNuclearWaste();
			bool nuclearWasteCreated = CreateNuclearWaste();
			if (!nuclearWasteCreated)
			{
				return;
			}

			Cell freeCell = Grid.GetRandomFreeCell();
			if (Grid.OccupyCell(_nuclearWaste, freeCell.GridPosition))
			{
				_nuclearWaste.SetPosition(freeCell.GridPosition);
			}
		}

		private bool CreateNuclearWaste()
		{
			// ladataan skene vasta, kun sitä tarvitaan ensimmäisen kerran.
			// Tätä kutsutaan lazy loadingiksi.
			if (_nuclearWasteScene == null)
			{
				_nuclearWasteScene = ResourceLoader.Load<PackedScene>(_nuclearWasteScenePath);
				if (_nuclearWasteScene == null)
				{
					GD.PrintErr("Can't load Nuclear Waste scene!");
					return false;
				}
			}

			_nuclearWaste = _nuclearWasteScene.Instantiate<NuclearWaste>();
			AddChild(_nuclearWaste);
			return true;
		}

		public void ReplaceApple()
		{
			DestroyApple();
			bool appleCreated = CreateApple();
			if (!appleCreated)
			{
				return;
			}

			Cell freeCell = Grid.GetRandomFreeCell();
			if (Grid.OccupyCell(_apple, freeCell.GridPosition))
			{
				_apple.SetPosition(freeCell.GridPosition);
			}
		}

		private bool CreateApple()
		{
			// ladataan omena-skene vasta, kun sitä tarvitaan ensimmäisen kerran.
			// Tätä kutsutaan lazy loadingiksi.
			if (_appleScene == null)
			{
				_appleScene = ResourceLoader.Load<PackedScene>(_appleScenePath);
				if (_appleScene == null)
				{
					GD.PrintErr("Can't load apple scene!");
					return false;
				}
			}

			_apple = _appleScene.Instantiate<Apple>();
			AddChild(_apple);
			return true;
		}

		private void DestroySpawns()
		{
			DestroyApple();
			DestroyNuclearWaste();
		}

		private void DestroyNuclearWaste()
		{
			if (_nuclearWaste != null)
			{
				Grid.ReleaseCell(_nuclearWaste.GridPosition);

				_nuclearWaste.QueueFree();
				_nuclearWaste = null;
			}
		}

		private void DestroyApple()
		{
			if (_apple != null)
			{
				Grid.ReleaseCell(_apple.GridPosition);

				_apple.QueueFree();
				_apple = null;
			}
		}

		public bool Load()
		{
			string savePath = ProjectSettings.GlobalizePath("user://");
			savePath = Path.Combine(savePath, Config.SaveFolder);
			string loadedJson = LoadFromFile(savePath, Config.QuickSaveFile);

			Json jsonLoader = new Json();
			Error loadError = jsonLoader.Parse(loadedJson);
			if (loadError != Error.Ok)
			{
				GD.PrintErr($"Virhe ladattaessa peliä! Virhe: {loadError}");
				return false;
			}

			Dictionary saveData = (Dictionary)jsonLoader.Data;
			Dictionary snakeData = (Dictionary)saveData["Snake"];
			Dictionary appleData = (Dictionary)saveData["Apple"];
			Dictionary nuclearWasteData = (Dictionary)saveData["NuclearWaste"];
			Score = (int)saveData["Score"];

			DestroySnake();
			DestroySpawns();

			if (!CreateApple())
			{
				GD.PrintErr("Omenan luonti epäonnistui!");
				return false;
			}

			if (!CreateNuclearWaste())
			{
				GD.PrintErr("Ydinjätteen luonti epäonnistui!");
				return false;
			}

			_snake = CreateSnake();
			AddChild(_snake);

			if (!_snake.Load(snakeData))
			{
				GD.PrintErr("Madon lataus epäonnistui!");
				return false;
			}

			if (!_apple.Load(appleData))
			{
				GD.PrintErr("Omenan lataus epäonnistui!");
				return false;
			}

			if (!_nuclearWaste.Load(nuclearWasteData))
			{
				GD.PrintErr("Ydinjätteen lataus epäonnistui!");
				return false;
			}

			return true;
		}

		public void Save()
		{
			Dictionary saveData = new Dictionary();
			saveData.Add("Score", Score);
			saveData.Add("Snake", _snake.Save());
			saveData.Add("Apple", _apple.Save());
			saveData.Add("NuclearWaste", _nuclearWaste.Save());

			string json = Json.Stringify(saveData);

			string savePath = ProjectSettings.GlobalizePath("user://");
			savePath = Path.Combine(savePath, Config.SaveFolder);

			if (SaveToFile(savePath, Config.QuickSaveFile, json))
			{
				GD.Print("Peli tallennettu!");
			}
			else
			{
				GD.PrintErr("Pelin tallennus epäonnistui!");
			}
		}

		#endregion

		public string LoadFromFile(string path, string fileName)
		{
			path = Path.Combine(path, fileName);

			if (!File.Exists(path))
			{
				GD.PrintErr($"Tiedostoa {path} ei löytynyt!");
				return null;
			}

			try
			{
				return File.ReadAllText(path);
			}
			catch (Exception e)
			{
				GD.PrintErr($"Virhe ladattaessa tiedostoa: {e.Message}");
				return null;
			}
		}

		private bool SaveToFile(string path, string fileName, string json)
		{
			if (!Directory.Exists(path))
			{
				try
				{
					Directory.CreateDirectory(path);
				}
				catch (Exception e)
				{
					GD.PrintErr($"Virhe tallennettaessa tiedostoa: {e.Message}");
					return false;
				}
			}

			path = Path.Combine(path, fileName);

			try
			{
				File.WriteAllText(path, json);
			}
			catch (Exception e)
			{
				GD.PrintErr($"Virhe tallennettaessa tiedostoa: {e.Message}");
				return false;
			}

			return true;
		}

		public void Pause()
		{
			if (_levelScene == null)
			{
				_levelScene = GetTree();
			}

			_levelScene.Paused = true;
		}

		public void Resume()
		{
			if (_levelScene == null)
			{
				_levelScene = GetTree();
			}

			_levelScene.Paused = false;
		}
	}
}