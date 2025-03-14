using System;
using System.Collections.Generic;
using Godot;

namespace SnakeGame
{
	public partial class SnakePart : Node2D, ICellOccupier
	{
		public enum SnakePartType
		{
			None = 0,
			Head,
			Body,
			Tail
		}

		public CellOccupierType Type { get { return CellOccupierType.Snake; } }

		[Export] public SnakePartType PartType { get; set; } = SnakePartType.None;
		[Export] private AnimationTree _animationTree = null;
		[Export] private double _minAnimTime = 0;
		[Export] private double _maxAnimTime = 1;
		private double _animTime = 0;

		// FSM = Finite State Machine, see https://gameprogrammingpatterns.com/state.html
		private AnimationNodeStateMachinePlayback _animationFSM = null;

		public Vector2I GridPosition { get; private set; }

		public Grid LevelGrid
		{
			get { return Level.Current.Grid; }
		}

		public bool UseAnimation
		{
			get { return _animationTree != null; }
		}

		// Madon rotaatio asteina.
		public static Dictionary<Snake.Direction, int> Directions = new Dictionary<Snake.Direction, int>()
		{
			{ Snake.Direction.Up, 0},
			{ Snake.Direction.Down, 180},
			{ Snake.Direction.Left, 270},
			{ Snake.Direction.Right, 90},
		};

		public override void _Ready()
		{
			if (UseAnimation)
			{
				_animationFSM = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
				if (_animationFSM != null)
				{
					_animationFSM.Start("Idle");
					_animTime = GD.RandRange(_minAnimTime, _maxAnimTime);
				}
			}
		}

		public override void _Process(double delta)
		{
			if (!UseAnimation)
			{
				return;
			}

			_animTime -= delta;
			if (_animTime <= 0)
			{
				_animationFSM.Travel("Lick");
				_animTime = GD.RandRange(_minAnimTime, _maxAnimTime);
			}
		}

		/// <summary>
		/// Asettaa madon osan gridPositionia vastaavaan sijaintiin levelissä. Varaa gridiltä sijaintia vastaavan solun.
		/// </summary>
		/// <param name="gridPosition">Sijainti gridin koodrinaatiostssa.</param>
		/// <returns>True, jos sijainnnin asettaminen onnistuu, false muuten.</returns>
		public bool SetPosition(Vector2I gridPosition)
		{
			if (LevelGrid.GetWorldPosition(gridPosition, out Vector2 worldPosition) &&
				LevelGrid.OccupyCell(this, gridPosition))
			{
				// Käännä madon osa oikein.
				Snake.Direction movingDirection = GetMovingDirection(GridPosition, gridPosition);
				if (movingDirection != Snake.Direction.None)
				{
					RotationDegrees = Directions[movingDirection];
				}

				// Aseta madon sijainti gridille ja leveliin
				GridPosition = gridPosition;
				Position = worldPosition;
				return true;
			}

			// Madon uusi sijainti ei ole gridillä (mato yrittää liikkua levelin ulkopuolelle) tai solun varaus
			// ei onnistu (este).
			return false;
		}

		private Snake.Direction GetMovingDirection(Vector2I originalPosition, Vector2I newPosition)
		{
			if (originalPosition.X < newPosition.X)
			{
				return Snake.Direction.Right;
			}
			if (originalPosition.X > newPosition.X)
			{
				return Snake.Direction.Left;
			}
			if (originalPosition.Y < newPosition.Y)
			{
				return Snake.Direction.Down;
			}
			if (originalPosition.Y > newPosition.Y)
			{
				return Snake.Direction.Up;
			}

			// Ei liikuta mihinkään.
			return Snake.Direction.None;
		}

		public Godot.Collections.Dictionary Save()
		{
			Godot.Collections.Dictionary saveData = new Godot.Collections.Dictionary();
			Godot.Collections.Dictionary positionData = new Godot.Collections.Dictionary()
			{
				{ "X", GridPosition.X },
				{ "Y", GridPosition.Y }
			};
			saveData.Add("Position", positionData);
			saveData.Add("Rotation", RotationDegrees);

			return saveData;
		}

		public bool Load(Godot.Collections.Dictionary saveData)
		{
			Godot.Collections.Dictionary positionData = (Godot.Collections.Dictionary)saveData["Position"];
			if (positionData != null)
			{
				Vector2I gridPosition = new Vector2I((int)positionData["X"], (int)positionData["Y"]);
				if (SetPosition(gridPosition))
				{
					RotationDegrees = (float)saveData["Rotation"];
					return true;
				}
			}

			return false;
		}
	}
}