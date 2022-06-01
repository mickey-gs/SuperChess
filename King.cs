using Godot;
using System;
using System.Collections.Generic;

public class King : Piece
{
	public override void SetColour(char col) {
		Colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "K.png"));
	}
	
	public override List<Vector2> Moves(Square[,] board, Vector2 origin) {
		List<Vector2> moves = new List<Vector2> {};
		for (int i = -1; i < 2; i++) {
			for (int j = -1; j < 2; j++) {
				try {
					if (board[(int)origin.x + i, (int) origin.y + j].GetPieceColour() != Colour) { 
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
