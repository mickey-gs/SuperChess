using Godot;
using System;

public class King : Piece
{
	public override void SetColour(char col) {
		colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "K.png"));
	}
}
