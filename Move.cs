using Godot;
using System;

public struct Move {
	public Square Origin;
	public Square Dest;
	public bool Check;
	public bool Capture;
	public bool Checkmate;
}
