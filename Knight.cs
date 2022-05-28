using Godot;
using System;

public class Knight : Piece {
	public override string Greet() {
		return "bananas~~";
	}
	
	public override void SetColour(char col) {
		colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "N.png"));
	}
}
