using Godot;
using System;

public class FENParser : Node2D {
	public static Square[,] Parse(string fen) {
		string[] splitFen = fen.Split("/");
		Square[,] output = new Square[8,8];
		var scene = GD.Load<PackedScene>("res://Square.tscn");
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				output[i,j] = (Square)scene.Instance();
			}
		}
		for (int i = 0; i < splitFen.Length; i++) {
			int j = 0;
			while (j < splitFen[i].Length) {
				if (int.TryParse(splitFen[i][j].ToString(), out int jump)) {
					j += jump;
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
					output[i,j].BestowPiece(pieceName, col);
					j++;
				}
			}
		}
		return output;
	}
}
