using Godot;
using System;

public class Square : Node2D {	
	public Vector2 Pos;
	
	public Square() {
		this.Pos = new Vector2(0, 0);
	}
	
	public Square(int x, int y) {
		this.Pos = new Vector2(x, y);
	}
	
	public override void _Ready() {
		var parent = (Board)GetParent<Node2D>();
		Connect("input_event", parent, "_on_Square_input_event");
	}
	
	public override void _Process(float delta) {
//		if (Input.IsActionPressed("ui_click")) {
//			EmitSignal(nameof(Clicked));
//			GD.Print(this.Pos);
//			EmitSignal("ClickedPlus", this.Pos);
//		}
	}
	
	public void SetPos(int x, int y) {
		this.Pos = new Vector2(x, y);
	}
	
	public void SetPos(Vector2 Pos) {
		this.Pos = Pos;
	}
	
	public void BestowPiece(Names name, char colour) {
		string pieceName = null;
		switch (name) {
			case Names.Pawn:
				pieceName = "Pawn";
				break;
			
			case Names.Bishop:
				pieceName = "Bishop";
				break;
				
			case Names.Knight:
				pieceName = "Knight";
				break;
				
			case Names.Rook:
				pieceName = "Rook";
				break;
				
			case Names.Queen:
				pieceName = "Queen";
				break;
				
			case Names.King:
				pieceName = "King";
				break;
		}
		var Scene = GD.Load<PackedScene>("res://" + pieceName + ".tscn");
		var piece = (Piece)Scene.Instance();
		piece.SetColour(colour);
		AddChild(piece);
	}
}
