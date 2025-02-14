using Godot;
using System;

namespace SnakeGame
{
	public partial class Apple : Collectable
	{
		[Export] private int _score = 10;

		public override void Collect(Snake snake)
		{
			// TODO: Kasvata matoa

			// Pisteitä ylläpidetään Levelissä. Kasvata Scorea _scoren verran.
			Level.Current.Score += _score;

			// Korvaa omena uudella eri sijainnissa
			Level.Current.ReplaceApple();

			// Korvaa myös negatiivinen ydinjäte.
			Level.Current.ReplaceNuclearWaste();
		}
	}
}