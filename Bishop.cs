using Godot;
using System;
using System.Collections.Generic;

public class Bishop : Piece {
	public override void SetColour(char col) {
		Colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "B.png"));
	}
	
	public override List<Vector2> Moves(Square[,] board, Vector2 origin) {
		var moves = GenMoves(board, origin, new Vector2(1, 1), new List<Vector2> {});
		moves = GenMoves(board, origin, new Vector2(1, -1), moves);
		moves = GenMoves(board, origin, new Vector2(-1, 1), moves);
		moves = GenMoves(board, origin, new Vector2(-1, -1), moves);
		return moves;
	}
}
