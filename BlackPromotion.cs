using Godot;
using System;

public class BlackPromotion : VBoxContainer {
	[Signal]
	public delegate void Promotion(string pieceName);
	[Signal]
	public delegate void Appear();
	
	public override void _Ready() {
		var parent = (Board)GetParent<Node2D>();
		Connect(nameof(Promotion), parent, "OnBlackPromotion");
		var root = (GameSpace)GetNode("/root/GameSpace");
		Connect(nameof(Appear), root, "OnPromotionMenuAppearing");
		EmitSignal(nameof(Appear));
	}

	private void _on_QueenBttn_pressed()
	{
		EmitSignal(nameof(Promotion), "Queen");
	}


	private void _on_RookBttn_pressed()
	{
		EmitSignal(nameof(Promotion), "Rook");
	}


	private void _on_BishopBttn_pressed()
	{
		EmitSignal(nameof(Promotion), "Bishop");
	}


	private void _on_KnightBttn_pressed()
	{
		EmitSignal(nameof(Promotion), "Knight");
	}
}
