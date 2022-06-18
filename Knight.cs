using Godot;
using System;
using System.Collections.Generic;

public class Knight : Piece {
	public Knight() {
		PieceName = Names.Knight;
	}
	
	public override void SetColour(char col) {
		Colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "N.png"));
	}
	
	public override List<Vector2> Moves(Square[,] board, Vector2 origin) {
		List<Vector2> moves = new List<Vector2> {};
		var buffer = GenMoves(board, origin, new Vector2(1, 2), new List<Vector2> {});
		if (buffer.Count > 0) 
			moves.Add(buffer[0]);
		buffer = GenMoves(board, origin, new Vector2(1, -2), new List<Vector2> {});
		if (buffer.Count > 0) 
			moves.Add(buffer[0]);
		buffer = GenMoves(board, origin, new Vector2(-1, 2), new List<Vector2> {});
		if (buffer.Count > 0) 
			moves.Add(buffer[0]);
		buffer = GenMoves(board, origin, new Vector2(-1, -2), new List<Vector2> {});
		if (buffer.Count > 0) 
			moves.Add(buffer[0]);
		buffer = GenMoves(board, origin, new Vector2(2, 1), new List<Vector2> {});
		if (buffer.Count > 0) 
			moves.Add(buffer[0]);
		buffer = GenMoves(board, origin, new Vector2(2, -1), new List<Vector2> {});
		if (buffer.Count > 0) 
			moves.Add(buffer[0]);
		buffer = GenMoves(board, origin, new Vector2(-2, 1), new List<Vector2> {});
		if (buffer.Count > 0) 
			moves.Add(buffer[0]);
		buffer = GenMoves(board, origin, new Vector2(-2, -1), new List<Vector2> {});
		if (buffer.Count > 0) 
			moves.Add(buffer[0]);
		return moves;
	}
}
