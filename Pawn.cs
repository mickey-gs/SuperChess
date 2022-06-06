using Godot;
using System;
using System.Collections.Generic;

public class Pawn : Piece {
	public override string Greet() {
		return $"I am a {Colour} pawn!";
	}
	
	public override void SetColour(char col) {
		Colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "p.png"));
	}
	
	public override List<Vector2> Moves(Square[,] board, Vector2 origin, Vector2 enPassant) {
		List<Vector2> moves = new List<Vector2> {};
		int dir = (Colour == 'w' ? 1 : -1);
		char targetCol = 'a';
		
		try {
			targetCol = board[(int)origin.x, (int)origin.y + dir * 1].GetPieceColour();
			if (targetCol == 'n')
				moves.Add(new Vector2(origin.x, origin.y + dir * 1));	
		}
		catch (System.IndexOutOfRangeException) {}
			
		try {
			targetCol = board[(int)origin.x, (int)origin.y + dir * 2].GetPieceColour();
			if (targetCol == 'n' && (((int)origin.y == 1 && Colour == 'w') || ((int)origin.y == 6 && Colour == 'b')))
				moves.Add(new Vector2(origin.x, origin.y + dir * 2));
		}
		catch (System.IndexOutOfRangeException) {}
		
		try {
			targetCol = board[(int)origin.x + 1, (int)origin.y + dir * 1].GetPieceColour();
			if ((targetCol != 'n' && targetCol != Colour) || 
				((int)origin.x + 1 == (int)enPassant.x && (int)origin.y + dir == (int)enPassant.y)) 
				moves.Add(new Vector2((int)origin.x + 1, (int)origin.y + dir));
		}
		catch (System.IndexOutOfRangeException) {}
		
		try {
			targetCol = board[(int)origin.x - 1, (int)origin.y + dir * 1].GetPieceColour();
			if ((targetCol != 'n' && targetCol != Colour) ||
				((int)origin.x - 1 == (int)enPassant.x && (int) origin.y + dir == (int)enPassant.y)) 
				moves.Add(new Vector2((int)origin.x - 1, (int)origin.y + dir * 1));
		}
		catch (System.IndexOutOfRangeException) {}
		return moves;
	}
}
