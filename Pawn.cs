using Godot;
using System;

public class Pawn : Piece {
	public override string Greet() {
		return $"I am a {colour} pawn!";
	}
	
	public override void SetColour(char col) {
		colour = col;
		var sprite = GetNode<Sprite>("Sprite");
		sprite.SetTexture(GD.Load<Texture>("./assets/" + col + "p.png"));
	}
}
