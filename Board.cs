using Godot;
using System;

public class Board : Node2D
{
	[Export]
	public Color LightSquares = new Color(1,1,1,(float)0.5);
	
	public Vector2 ScreenSize;
	private Square[,] squares = new Square[8,8];
	private char Turn = 'w';
	private Square Selected = null;
//
//	#pragma warning disable 649
//	[Export]
//	public PackedScene PieceScene;
//	#pragma warning restore 649

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ScreenSize = GetViewportRect().Size;
		char col = 'w';
		var scene = GD.Load<PackedScene>("res://Square.tscn");
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				squares[i,j] = (Square)scene.Instance();;
				if (i > 2) {
					col = 'b';
				}
				if (i == 1 || i == 6) {
					squares[i,j].BestowPiece(Names.Pawn, col);
				}
				else if (i == 0 || i == 7) {
					if (j == 0 || j == 7) {
						squares[i,j].BestowPiece(Names.Rook, col);
					}
					else if (j == 1 || j == 6) {
						squares[i,j].BestowPiece(Names.Knight, col);
					}
					else if (j == 2 || j == 5) {
						squares[i,j].BestowPiece(Names.Bishop, col);
					}
					else if (j == 3) {
						squares[i,j].BestowPiece(Names.Queen, col);
					}
					else if (j == 4) {
						squares[i,j].BestowPiece(Names.King, col);
					}
				}
				squares[i, j].SetPos(j, i);
				AddChild(squares[i,j]);
				squares[i,j].Position = 
					new Vector2((ScreenSize.x / 8) * j, ScreenSize.y - (ScreenSize.y / 8) * (i + 1));
				if ((i + j) % 2 != 0) {
					squares[i,j].GetNode<Polygon2D>("Sprite").Color = LightSquares;
				}
				//GD.Print(squares[i,j].GetNode("Sprite"));
			}
		}
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
			if (squares[rank, file].GetPieceColour() != Turn)
				return;
				
			var p = (Piece)squares[rank, file].GetChildren()[3];
			squares[rank, file].Highlight();
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
