using Godot;
using System;
using System.Collections.Generic;

public class King : Piece
{
	public King() {
		PieceName = Names.King;
	}
	
	public override void SetColour(char col) {
		Colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "K.png"));
	}
	
	public override List<Vector2> LegalMoves(Square[,] squares, Vector2 origin, Board board) {
		var allDests = Moves(squares, origin, board);
		int homeRank = (Colour == 'w' ? 0 : 7);
		if (board.CastleKingside(this)) {
			allDests.Add(new Vector2(6,homeRank));
		}
		if (board.CastleQueenside(this)) {
			allDests.Add(new Vector2(2, homeRank));
		}
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
			if (illegal) {
				allDests[i] = new Vector2(-1,-1);
			}
			squares[(int)origin.x,(int)origin.y].BestowPiece(originPieceName, originCol);
		}
		
		allDests.RemoveAll(sq => (int)sq.x == -1);
		
		return allDests;
	}
	
	public override List<Vector2> Moves(Square[,] squares, Vector2 origin, Board board) {
		List<Vector2> moves = new List<Vector2> {};
		for (int i = -1; i < 2; i++) {
			for (int j = -1; j < 2; j++) {
				try {
					if (squares[(int)origin.x + i, (int) origin.y + j].GetPieceColour() != Colour) { 
						moves.Add(new Vector2((int) origin.x + i, (int) origin.y + j));
					}
				}
				catch (System.IndexOutOfRangeException e) {
					continue;
				}
			}
		}
		return moves;
	}
}
