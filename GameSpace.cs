using Godot;
using System;

public class GameSpace : Area2D
{
	private string Fen = "r1bqkb1r/pppppppp/8/8/8/8/PPPPPPPP/R1BQKB1R b KQkq - 1 0";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		var scene = GD.Load<PackedScene>("res://Board.tscn");
		Board board = (Board)scene.Instance();
		board = FENParser.Parse(Fen);
		AddChild(board);
	}
	
	public float BoardWidth() {
		var rectangle = (CollisionShape2D)GetNode("CollisionShape2D");
		var shape = (RectangleShape2D)rectangle.Shape;
		return shape.GetExtents().x;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
