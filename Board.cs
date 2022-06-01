using Godot;
using System;

public class Board : Node2D
{
	[Export]
	public Color LightSquares = new Color(1,1,1,(float)0.5);
	
	public Vector2 ScreenSize;
	private Square[,] squares;
	private char Turn = 'w';
	private Square Selected = null;
	private string Fen = "rnbqkbn1/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

	public const string StandardFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
//
//	#pragma warning disable 649
//	[Export]
//	public PackedScene PieceScene;
//	#pragma warning restore 649

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ScreenSize = GetViewportRect().Size;
		squares = FENParser.Parse(Fen);
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				AddChild(squares[i,j]);
				squares[j,i].SetPos(new Vector2(i, j));
				squares[j,i].Position = 
					new Vector2((ScreenSize.x / 8) * j, ScreenSize.y - (ScreenSize.y / 8) * (i + 1));
				if ((i + j) % 2 != 0) {
					squares[i,j].GetNode<Polygon2D>("Sprite").Color = LightSquares;
				}
			}
		}
//		for (int i = 0; i < 8; i++) {
//			string collect = "";
//			for (int j = 0; j < 8; j++) {
//				collect += squares[i,j].GetPieceName();	
//			}
//			GD.Print(collect);
//		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}

	private void setSize() {
		ScreenSize = GetViewportRect().Size;
		//var sprite = GetNode<Sprite>("Sprite");
	}
	
	private void Clear() {
		var children = GetChildren();
		for (int i = 0; i < children.Count; i++) {
			if (children[i] is Square sq) {
				sq.Clear();
			}
		}
	}
	
	private void Selection(Vector2 position) {
		Clear();
		
		int file = (int)((position.x / 500) * 8);
		int rank = (int)(((500 - position.y) / 500) * 8);
			
		try {
			if (squares[file, rank].GetPieceColour() != Turn)
				return;
				
			var p = (Piece)squares[file, rank].GetChildren()[3];
			squares[file, rank].Highlight();
			foreach (var dest in p.Moves(squares, new Vector2(file, rank))) {
				GD.Print(dest);
			}
		}
		catch (System.IndexOutOfRangeException) {
		}
	}
	
	private void _on_Square_input_event(object viewport, object @event, int shape_idx)
	{
		if (@event is InputEventMouseButton e) {
			if (e.Pressed && e.ButtonIndex == 1) {
				Selection(e.Position);
			}
		}
	}
}
