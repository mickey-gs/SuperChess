using Godot;
using System;
using System.Collections.Generic;

public class Piece : Node2D
{
	public char Colour;
	protected Names PieceName;
	
	public virtual void SetColour(char col) {
	}
	
	public virtual Names GetName() {
		return PieceName;
	}
	
	public virtual List<Vector2> Moves(Square[,] board, Vector2 origin) {
		return new List<Vector2> { new Vector2(origin.x + 1, origin.y + 2) };
	}
	
	public List<Vector2> LegalMoves(Square[,] board, Vector2 origin, Vector2 enPassant) {
		var allDests = Moves(board, origin, enPassant);
		for (int i = 0; i < allDests.Count; i++) {
			Names originPieceName = board[(int)origin.x,(int)origin.y].GetPieceName();
			char originCol = board[(int)origin.x,(int)origin.y].GetPieceColour();
			Names destPieceName = board[(int)allDests[i].x,(int)allDests[i].y].GetPieceName();
			char destCol = board[(int)allDests[i].x,(int)allDests[i].y].GetPieceColour();
			board[(int)allDests[i].x,(int)allDests[i].y].BestowPiece(originPieceName, originCol);
			board[(int)origin.x,(int)origin.y].RemovePiece();
			bool illegal = (new Board(board)).LookForChecks(Colour);
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
	
	public virtual List<Vector2> LegalMoves(Square[,] squares, Vector2 origin, Board board) {
		var allDests = Moves(squares, origin, board.EnPassantSq);
		for (int i = 0; i < allDests.Count; i++) {
			Names originPieceName = squares[(int)origin.x,(int)origin.y].GetPieceName();
			char originCol = squares[(int)origin.x,(int)origin.y].GetPieceColour();
			Names destPieceName = squares[(int)allDests[i].x,(int)allDests[i].y].GetPieceName();
			char destCol = squares[(int)allDests[i].x,(int)allDests[i].y].GetPieceColour();
			squares[(int)allDests[i].x,(int)allDests[i].y].BestowPiece(originPieceName, originCol);
			squares[(int)origin.x,(int)origin.y].RemovePiece();
			bool illegal = (new Board(squares)).LookForChecks(Colour);
			squares[(int)allDests[i].x,(int)allDests[i].y].RemovePiece();
			if (destCol != 'n') {
				squares[(int)allDests[i].x,(int)allDests[i].y].BestowPiece(destPieceName, destCol);
			}
			squares[(int)origin.x,(int)origin.y].BestowPiece(originPieceName, originCol);
			if (illegal) 
				allDests[i] = new Vector2(-1, -1);
		}
		
		allDests.RemoveAll(sq => (int)sq.x == -1);
		return allDests;
	}
	
	public virtual List<Vector2> Moves(Square[,] board, Vector2 origin, Vector2 enPassant) {
		return Moves(board, origin);
	}
	
	public virtual List<Vector2> Moves(Square[,] squares, Vector2 origin, Board board) {
		return Moves(squares, origin, board.EnPassantSq);
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
