using Godot;
using System;

public class FENParser : Node2D {
	public static Board Parse(string fen) {
		string[] setup = fen.Split(" ");
		char turn = setup[1][0];
		string rights = setup[2];
		bool[] castlingRights = {rights.Contains("Q"), rights.Contains("K"),
			rights.Contains("q"), rights.Contains("k")};
		Vector2 enPassantSq = Square.NotationToPos(setup[3]);
		int[] halfmoves = {int.Parse(setup[4]), int.Parse(setup[5].ToString())};
		
		string[] splitFen = setup[0].Split("/");
		Square[,] output = new Square[8,8];
		var scene = GD.Load<PackedScene>("res://Square.tscn");
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				output[i,j] = (Square)scene.Instance();
			}
		}
		for (int i = 0; i < 8; i++) {
			int pointer = 0;
			for (int j = 0; j < splitFen[i].Length; j++) {
				if (int.TryParse(splitFen[i][j].ToString(), out int jump)) {
					pointer += jump;
				}
				else {
					Names pieceName = Names.King;
					char col = 'a';
					switch (splitFen[i][j].ToString().ToUpper()) {
						case "R":
							pieceName = Names.Rook;
							break;
							
						case "B":
							pieceName = Names.Bishop;
							break;
							
						case "N":
							pieceName = Names.Knight;
							break;
							
						case "Q":
							pieceName = Names.Queen;
							break;
							
						case "K":
							pieceName = Names.King;
							break;
							
						case "P":
							pieceName = Names.Pawn;
							break;
					}
					if (splitFen[i][j].ToString().ToUpper() == splitFen[i][j].ToString()) {
						col = 'w';
					}
					else {
						col = 'b';
					}
					output[pointer,7 - i].BestowPiece(pieceName, col);
					pointer++;
				}
			}
		}
		return new Board(output, turn, castlingRights, enPassantSq, halfmoves);
	}
}
