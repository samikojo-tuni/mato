using Godot;
using SnakeGame.UI;
using System;

namespace SnakeGame
{
	public partial class Level : Node2D
	{
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

		// Sceneviittaukset, joista voidaan luoda olioita.
		private PackedScene _snakeScene = null;
		private PackedScene _appleScene = null;
		private PackedScene _nuclearWasteScene = null;

		private int _score = 0;

		private Grid _grid = null;

		// Varsinaiset viittaukset Level-skenessä oleviin olioihin.
		private Snake _snake = null;
		// Omenoita voi olla olemassa yksi kerrallaan.
		private Apple _apple = null;
		private NuclearWaste _nuclearWaste = null;

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

			ResetGame();
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

			// Nollaa pisteet
			Score = 0;

			// Luo omena
			ReplaceApple();
		}

		public void DestroySnake()
		{
			if (_snake != null)
			{
				// Vapauta madon solu gridillä.
				Vector2I snakePosition = _snake.GridPosition;
				Grid.ReleaseCell(snakePosition);

				_snake.QueueFree();
				_snake = null;
			}
		}

		public void ReplaceNuclearWaste()
		{
			if (_nuclearWaste != null)
			{
				Grid.ReleaseCell(_nuclearWaste.GridPosition);

				_nuclearWaste.QueueFree();
				_nuclearWaste = null;
			}

			// ladataan skene vasta, kun sitä tarvitaan ensimmäisen kerran.
			// Tätä kutsutaan lazy loadingiksi.
			if (_nuclearWasteScene == null)
			{
				_nuclearWasteScene = ResourceLoader.Load<PackedScene>(_nuclearWasteScenePath);
				if (_nuclearWasteScene == null)
				{
					GD.PrintErr("Can't load Nuclear Waste scene!");
					return;
				}
			}

			_nuclearWaste = _nuclearWasteScene.Instantiate<NuclearWaste>();
			AddChild(_nuclearWaste);

			Cell freeCell = Grid.GetRandomFreeCell();
			if (Grid.OccupyCell(_nuclearWaste, freeCell.GridPosition))
			{
				_nuclearWaste.SetPosition(freeCell.GridPosition);
			}
		}

		public void ReplaceApple()
		{
			if (_apple != null)
			{
				Grid.ReleaseCell(_apple.GridPosition);

				_apple.QueueFree();
				_apple = null;
			}

			// ladataan omena-skene vasta, kun sitä tarvitaan ensimmäisen kerran.
			// Tätä kutsutaan lazy loadingiksi.
			if (_appleScene == null)
			{
				_appleScene = ResourceLoader.Load<PackedScene>(_appleScenePath);
				if (_appleScene == null)
				{
					GD.PrintErr("Can't load apple scene!");
					return;
				}
			}

			_apple = _appleScene.Instantiate<Apple>();
			AddChild(_apple);

			Cell freeCell = Grid.GetRandomFreeCell();
			if (Grid.OccupyCell(_apple, freeCell.GridPosition))
			{
				_apple.SetPosition(freeCell.GridPosition);
			}
		}

		#endregion
	}
}