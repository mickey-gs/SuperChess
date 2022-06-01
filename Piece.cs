using Godot;
using System;
using System.Collections.Generic;

public class Piece : Node2D
{
	public char Colour;

	public virtual string Greet() {
		return "hello!";
	}
	
	public virtual void SetColour(char col) {
	}
	
	public virtual List<Vector2> Moves(Square[,] board, Vector2 origin) {
		return new List<Vector2> { new Vector2(origin.x + 1, origin.y + 2) };
	}
	
	protected virtual List<Vector2> GenMoves(Square[,] board, Vector2 origin, 
		Vector2 dir, List<Vector2> squares) {
			Vector2 dest = new Vector2(origin.x + dir.x, origin.y + dir.y);
		try {
			if (board[(int)dest.x, (int)dest.y].GetPieceColour() != Colour) {
				squares.Add(dest);
				if (board[(int)dest.x, (int)dest.y].GetPieceColour() != 'n')
					return GenMoves(board, dest, dir, squares);
				else
					return squares;
			}
			else {
				return squares;
			}
		}
		catch (System.IndexOutOfRangeException e) {
			return squares;
		}
	}
}
