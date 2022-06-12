using Godot;
using System;

public class StartButton : Button	{
	public override void _Ready() {
		var root = (GameSpace)GetNode("/root/GameSpace");
		var fen = (LineEdit)((VBoxContainer)GetParent()).GetNode("StartFEN");
		Connect("pressed", root, "_on_StartButton_pressed", new Godot.Collections.Array {fen});
	}
}
