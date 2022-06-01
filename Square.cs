using Godot;
using System;

public class Square : Node2D {	
	public Vector2 Pos;
	public static Color DARK_SQUARE = new Color(0, 0, 0, (float)0.63);
	public static Color LIGHT_SQUARE = new Color(0, 0, 0, 0);
	
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
	
	public void Highlight() {
		var sprite = (Polygon2D)GetNode("Sprite");
		sprite.Color = new Color((float)0.92, 1, 0, (float)1);
	}
	
	public void Clear() {
		GetNode<Polygon2D>("Sprite").Color = ((Pos.x + Pos.y) % 2 == 0 ? 
			DARK_SQUARE : LIGHT_SQUARE);
	}
	
	public char GetPieceColour() {
		try {
			var piece = (Piece)GetChildren()[3];
			return piece.Colour;
		}
		catch (System.IndexOutOfRangeException e) {
			return 'n';
		}
	}
	
	public string GetPieceName() {
		try {
			var piece = (Piece)GetChildren()[3];
			return piece.GetType().ToString();
		}
		catch (System.IndexOutOfRangeException e) {
			return "empty";
		}
	}
	
	public string GetPieceName() {
		try {
			var piece = (Piece)GetChildren()[3];
			return piece.GetType().ToString();
		}
		catch (System.IndexOutOfRangeException e) {
			return "empty";
		}
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
		var sprite = piece.GetNode<Sprite>("Sprite");
		sprite.Scale = new Vector2((float)0.9, (float)0.9);
		sprite.Position = new Vector2(sprite.Position.x + 1, sprite.Position.y);
		AddChild(piece);
	}
}
