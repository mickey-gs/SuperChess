using Godot;
using System;
using System.Collections.Generic;

public class Rook : Piece {
	public override void SetColour(char col) {
		Colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "R.png"));
	}
	
	public override List<Vector2> Moves(Square[,] board, Vector2 origin) {
		return GenMoves(board, origin, new Vector2(1, 0), new List<Vector2> {});
	} 
}
