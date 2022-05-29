using Godot;
using System;

public class Piece : Node2D
{
	public char Colour;

	public virtual string Greet() {
		return "hello!";
	}
	
	public virtual void SetColour(char col) {
	}
}
