using Godot;
using System;

public class Square : Node2D {	
	public Vector2 Pos;
	public static Color DARK_SQUARE = new Color(0, 0.5F, 0, 1);
	public static Color LIGHT_SQUARE = new Color(0.95F, 0.95F, 1, 1);
	public static Color DARK_HIGHLIGHT = new Color(0.1F, 0.3F, 0.1F, 1);
	public static Color LIGHT_HIGHLIGHT = new Color(0.6F, 1, 0.6F, 1);
	public static Color RED_HIGHLIGHT = new Color(0.9F, 0, 0, 1);
	
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
	
	public void SetPos(int x, int y) {
		this.Pos = new Vector2(x, y);
		GetNode<Polygon2D>("Sprite").Color = ((int)(Pos.x + Pos.y) % 2 == 0 ? 
			DARK_SQUARE : LIGHT_SQUARE);
	}
	
	public void SetPos(Vector2 Pos) {
		GetNode<Polygon2D>("Sprite").Color = ((int)(Pos.x + Pos.y) % 2 == 0 ? 
			DARK_SQUARE : LIGHT_SQUARE);
		this.Pos = Pos;
	}
	
	public void Highlight() {
		GetNode<Polygon2D>("Sprite").Color = ((int)(Pos.x + Pos.y) % 2 == 0 ? 
			DARK_HIGHLIGHT : LIGHT_HIGHLIGHT);
	}
	
	public void RedHighlight() {
		GetNode<Polygon2D>("Sprite").Color = RED_HIGHLIGHT;
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
	
	public Names GetPieceName() {
		try {
			var piece = (Piece)GetChildren()[3];
			return piece.GetName();
		}
		catch (System.IndexOutOfRangeException e) {
			return Names.None;
		}
	}
	
	public static Vector2 NotationToPos(string notated) {
		if (notated[0] == '-') 
			return new Vector2(-1, -1);

		return new Vector2((int)(notated[0] - 1), (int)notated[1]);
	}
	
	public string GetPosNotation() {
		string ret = "";
		ret += (char)('a' + Pos.x);
		ret += ((int)Pos.y + 1).ToString();
		return ret;
	}
	
	public void BestowPiece(Names name, char colour) {
		RemovePiece();
		
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
		sprite.Scale = new Vector2(0.9F, 0.9F);
		sprite.Position = new Vector2(sprite.Position.x + 1, sprite.Position.y);
		AddChild(piece);
	}
	
	public void BestowPiece(string name, char colour) {
		RemovePiece();
			
		var Scene = GD.Load<PackedScene>("res://" + name + ".tscn");
		var piece = (Piece)Scene.Instance();
		piece.SetColour(colour);
		var sprite = piece.GetNode<Sprite>("Sprite");
		sprite.Scale = new Vector2(0.9F, 0.9F);
		sprite.Position = new Vector2(sprite.Position.x + 1, sprite.Position.y);
		AddChild(piece);
	}
	
	public void RemovePiece() {
		try {
			var pingus = (Node)GetChildren()[3];
			RemoveChild(pingus);
		}
		catch (System.IndexOutOfRangeException) {}
	}
}
