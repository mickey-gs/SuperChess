using Godot;
using System;

public class Queen : Piece {
	public override void SetColour(char col) {
		Colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "Q.png"));
	}
}
