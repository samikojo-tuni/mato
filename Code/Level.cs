using Godot;
using System;

namespace SnakeGame
{
	public partial class Level : Node2D
	{
		// Static on luokan ominaisuus, ei siitä luotujen ominaisuuksien.
		// Kaikki luokasta luodut olio jakavat saman staattisen muuttujan.
		private static Level _current = null;
		public static Level Current
		{
			get { return _current; }
		}

		[Export] private string _snakeScenePath = "res://Character/Snake.tscn";
		[Export] private string _appleScenePath = "res://Levels/Collectables/Apple.tscn";
		private PackedScene _snakeScene = null;
		private PackedScene _appleScene = null;
		private int _score = 0;
		private Grid _grid = null;
		private Snake _snake = null;
		// Omenoita voi olla olemassa yksi kerrallaan.
		private Apple _apple = null;

		public int Score
		{
			get { return _score; }
			set { _score = value; }
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

		/// <summary>
		/// Aloittaa uuden pelin.
		/// </summary>
		public void ResetGame()
		{
			// Tuhoa edellinen mato, jos se on olemassa.
			if (_snake != null)
			{
				_snake.QueueFree();
				_snake = null;
			}

			// Luo mato
			_snake = CreateSnake();
			AddChild(_snake);

			// Nollaa pisteet
			Score = 0;

			// Luo omena
			ReplaceApple();
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

		public void ReplaceApple()
		{
			if (_apple != null)
			{
				Grid.ReleaseCell(_apple.GridPosition);

				_apple.QueueFree();
				_apple = null;
			}

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
	}
}