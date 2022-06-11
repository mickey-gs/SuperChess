using Godot;
using System;

public class GameSpace : Area2D
{
	private string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 1 0";

	public override void _Ready() {
		var scene = GD.Load<PackedScene>("res://Board.tscn");
		Board board = (Board)scene.Instance();
		board = FENParser.Parse(Fen);
		AddChild(board);
	}
	
	public float BoardWidth() {
		var rectangle = (CollisionShape2D)GetNode("CollisionShape2D");
		var shape = (RectangleShape2D)rectangle.Shape;
		return shape.GetExtents().x * 2;
	}
}
