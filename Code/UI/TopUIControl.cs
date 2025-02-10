using Godot;
using System;

namespace SnakeGame.UI
{
	public partial class TopUIControl : Node
	{
		[Export] private Label _scoreLabel = null;

		public void SetScore(int score)
		{
			_scoreLabel.Text = $"Pisteet: {score}";
		}
	}
}