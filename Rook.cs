using Godot;
using System;
using System.Collections.Generic;

public class Rook : Piece {
	public override void SetColour(char col) {
		Colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "R.png"));
	}
	
	public override List<Vector2> Moves(Square[,] board, Vector2 origin) {
		var moves = GenMoves(board, origin, new Vector2(1, 0), new List<Vector2> {});
		GD.Print(moves.Count);
		moves.AddRange(GenMoves(board, origin, new Vector2(-1, 0), moves));
		moves.AddRange(GenMoves(board, origin, new Vector2(0, 1), moves));
		moves.AddRange(GenMoves(board, origin, new Vector2(0, -1), moves));
		return moves;
	} 
}
