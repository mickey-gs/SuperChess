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
	
	public List<Vector2> LegalMoves(Square[,] board, Vector2 origin) {
		var allDests = Moves(board, origin);
		for (int i = 0; i < allDests.Count; i++) {
			string originPieceName = board[(int)origin.x,(int)origin.y].GetPieceName();
			char originCol = board[(int)origin.x,(int)origin.y].GetPieceColour();
			string destPieceName = board[(int)allDests[i].x,(int)allDests[i].y].GetPieceName();
			char destCol = board[(int)allDests[i].x,(int)allDests[i].y].GetPieceColour();
			board[(int)allDests[i].x,(int)allDests[i].y].BestowPiece(originPieceName, originCol);
			board[(int)origin.x,(int)origin.y].RemovePiece();
			bool illegal = Board.LookForChecks(board, Colour);
			board[(int)allDests[i].x,(int)allDests[i].y].RemovePiece();
			if (destCol != 'n') {
				board[(int)allDests[i].x,(int)allDests[i].y].BestowPiece(destPieceName, destCol);
			}
			board[(int)origin.x,(int)origin.y].BestowPiece(originPieceName, originCol);
			if (illegal) 
				allDests[i] = new Vector2(-1, -1);
		}
		
		allDests.RemoveAll(sq => (int)sq.x == -1);
		return allDests;
	}
	
	protected virtual List<Vector2> GenMoves(Square[,] board, Vector2 origin, 
		Vector2 dir, List<Vector2> squares) {
		Vector2 dest = new Vector2(origin.x + dir.x, origin.y + dir.y);
		try {
			if (board[(int)dest.x, (int)dest.y].GetPieceColour() != Colour) {
				squares.Add(dest);
				if (board[(int)dest.x, (int)dest.y].GetPieceColour() == 'n') {
					return GenMoves(board, dest, dir, squares);
				}
				else {
					return squares;
				}
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
