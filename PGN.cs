using Godot;
using System;

public class PGN : Node {
	private string Game = "";
	
	public static string NotateMove(Move move) {
		string checkString = (move.Checkmate ? "#" : (move.Check ? "+" : ""));
		if (move.Dest.GetPieceName() == Names.King && Math.Abs(move.Origin.Pos.x - move.Dest.Pos.x) > 1) {
			return move.Dest.Pos.x == 2 ? "O-O-O" + checkString : "O-O" + checkString;
		}
		if (move.Capture && move.Dest.GetPieceName() == Names.Pawn) {
			return PosToNotation(move.Origin.Pos)[0].ToString() + "x" + PosToNotation(move.Dest.Pos) + checkString;
		}
		string pieceInitial = GetPieceInitial(move.Dest.GetPieceName());
		return pieceInitial + (move.Capture ? "x" : "") + PosToNotation(move.Dest.Pos) + checkString;
	}
	
	private static string GetPieceInitial(Names name) {
		if (name == Names.Knight) 
			return "N";
			
		if (name == Names.Pawn) 
			return "";
		
		string nameString = name.ToString();
		return nameString[0].ToString();
	}
	
	private static string PosToNotation(Vector2 pos) {
		string ret = "";
		ret += (char)('a' + pos.x);
		ret += ((int)pos.y + 1).ToString();
		return ret;
	}
}
