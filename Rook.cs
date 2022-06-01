using Godot;
using System;
using System.Collections.Generic;

public class Rook : Piece {
//	public override void _Ready() {
//		var squares = FENParser.Parse("r7/8/8/8/8/8/8/8");
//		foreach (var sq in Moves(squares, new Vector2(0, 7))) {
//			GD.Print(sq);
//		}
//	}
	
	public override void SetColour(char col) {
		Colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "R.png"));
	}
	
	public override List<Vector2> Moves(Square[,] board, Vector2 origin) {
		var moves = GenMoves(board, origin, new Vector2(1, 0), new List<Vector2> {});
		moves = (GenMoves(board, origin, new Vector2(-1, 0), moves));
		moves = (GenMoves(board, origin, new Vector2(0, 1), moves));
		moves = (GenMoves(board, origin, new Vector2(0, -1), moves));
		return moves;
	} 
}
