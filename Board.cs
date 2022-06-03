using Godot;
using System;
using System.Collections.Generic;

public class Board : Node2D
{
	[Export]
	public Color LightSquares = new Color(1,1,1,(float)0.5);
	
	public Vector2 ScreenSize;
	private Square[,] squares;
	private char Turn = 'w';
	private Square Selected = null;
	private string Fen = "N6r/8/8/6K1/3Q3r/1P1P2PP/P2N4/6bk";

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
				squares[j,i].SetPos(new Vector2(j, i));
				squares[j,i].Position = 
					new Vector2((ScreenSize.x / 8) * j, ScreenSize.y - (ScreenSize.y / 8) * (i + 1));
				if ((i + j) % 2 != 0) {
					squares[i,j].GetNode<Polygon2D>("Sprite").Color = LightSquares;
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}
	
	private static List<Vector2> AllAttacks(Square[,] board, char col) {
		List<Vector2> allAttacks = new List<Vector2> {};
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (board[i, j].GetPieceColour() == col) {
					try {
						var p = (Piece)board[i, j].GetChildren()[3];
						allAttacks.AddRange(p.Moves(board, board[i, j].Pos));
					}
					catch (System.IndexOutOfRangeException) {}
				}
			}
		}
		
		return allAttacks;
	}
	
	public static bool LookForChecks(Square[,] board, char col) {
		Square kingSquare = null;
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (board[i,j].GetPieceName() == "King" && board[i,j].GetPieceColour() == col)
					kingSquare = board[i,j];
			}
		}
		char enemyCol = (col == 'w' ? 'b' : 'w');
		return AllAttacks(board, enemyCol).Contains(kingSquare.Pos);
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
			if (Selected == null) {
				if (squares[file, rank].GetPieceColour() != Turn)
					return;
				
				Selected = squares[file, rank];
				var p = (Piece)squares[file, rank].GetChildren()[3];
				squares[file, rank].Highlight();
				foreach (var dest in p.LegalMoves(squares, new Vector2(file, rank))) {
					squares[(int)dest.x, (int)dest.y].Highlight();
				}
			}
			else {
				if (squares[file, rank].GetPieceColour() == Turn) {
					Selected = squares[file, rank];
					var p = (Piece)squares[file, rank].GetChildren()[3];
					squares[file, rank].Highlight();
					foreach (var dest in p.LegalMoves(squares, new Vector2(file, rank))) {
						squares[(int)dest.x, (int)dest.y].Highlight();
					}
				}
				else {
					var p = (Piece)Selected.GetChildren()[3];
					if (p.LegalMoves(squares, Selected.Pos).Contains(new Vector2(file, rank))) {
						squares[file, rank].BestowPiece(Selected.GetPieceName(), Turn);
						Turn = (Turn == 'w' ? 'b' : 'w');
						Selected.RemovePiece();
					}
				}
			}
			
			
		}
		catch (System.IndexOutOfRangeException) {}
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
