using Godot;
using System;
using System.Collections.Generic;

public class Board : Node2D
{
	[Export]
	public Color LightSquares = new Color(1,1,1,(float)0.5);
	
	public Vector2 ScreenSize;
	private Square[,] squares;
	public static Vector2 EnPassantSq = new Vector2(-1, -1);
	private char Turn = 'w';
	private Square Selected = null;
	private string Fen = StandardFEN;
	private int PromotionFile = 9;
	private bool Active = true;

	public const string StandardFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
//
//	#pragma warning disable 649
//	[Export]
//	public PackedScene PieceScene;
//	#pragma warning restore 649

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		var parent = (GameSpace)GetParent();
		float width = parent.BoardWidth();
		squares = FENParser.Parse(Fen);
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				AddChild(squares[i,j]);
				squares[j,i].SetPos(new Vector2(j, i));
				squares[j,i].Position = 
					new Vector2((width / 8) * j, width - (width / 8) * (i + 1));
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
						allAttacks.AddRange(p.Moves(board, board[i, j].Pos, EnPassantSq));
					}
					catch (System.IndexOutOfRangeException) {}
				}
			}
		}
		
		return allAttacks;
	}
	
	private static List<Vector2> AllMoves(Square[,] board, char col) {
		List<Vector2> allMoves = new List<Vector2> {};
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (board[i, j].GetPieceColour() == col) {
					try {
						var p = (Piece)board[i, j].GetChildren()[3];
						allMoves.AddRange(p.LegalMoves(board, board[i, j].Pos, EnPassantSq));
					}
					catch (System.IndexOutOfRangeException) {}
				}
			}
		}
		
		return allMoves;
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
		
		if (!Active)
			return;
		
		int file = (int)((position.x / 500) * 8);
		int rank = (int)(((500 - position.y) / 500) * 8);
			
		try {	
			if (Selected == null) {
				if (squares[file, rank].GetPieceColour() != Turn)
					return;
				
				Selected = squares[file, rank];
				var p = (Piece)squares[file, rank].GetChildren()[3];
				squares[file, rank].Highlight();
				foreach (var dest in p.LegalMoves(squares, new Vector2(file, rank), EnPassantSq)) {
					squares[(int)dest.x, (int)dest.y].Highlight();
				}
			}
			else {
				if (squares[file, rank].GetPieceColour() == Turn) {
					HighlightMoves(file, rank);
				}
				else {
					HandleMove(file, rank);
					CheckForEnd();
					LookForPromotion();
				}
			}
			
			
		}
		catch (System.IndexOutOfRangeException) {}
	}
	
	private void HighlightMoves(int file, int rank) {
		Selected = squares[file, rank];
		var p = (Piece)squares[file, rank].GetChildren()[3];
		squares[file, rank].Highlight();
		foreach (var dest in p.LegalMoves(squares, new Vector2(file, rank), EnPassantSq)) {
			squares[(int)dest.x, (int)dest.y].Highlight();
		}
	}
	
	private void HandleMove(int file, int rank) {
		var p = (Piece)Selected.GetChildren()[3];
		if (p.LegalMoves(squares, Selected.Pos, EnPassantSq).Contains(new Vector2(file, rank))) {
			if (file == (int)EnPassantSq.x && rank == (int)EnPassantSq.y
				&& Selected.GetPieceName() == "Pawn") {
				squares[file, rank + (Turn == 'w' ? -1 : 1)].RemovePiece();
			}
			EnPassantSq = new Vector2(-1, -1);
			squares[file, rank].BestowPiece(Selected.GetPieceName(), Turn);
			if (Selected.GetPieceName() == "Pawn" && Math.Abs(Selected.Pos.y - rank) > 1)
				EnPassantSq = new Vector2(file, rank + (Turn == 'w' ? -1 : 1));
			Turn = (Turn == 'w' ? 'b' : 'w');
			Selected.RemovePiece();
		}
	}
	
	private void LookForPromotion() {
		for (int i = 0; i < 8; i++) {
			if (squares[i,7].GetPieceName() == "Pawn") {
				Active = false;
				squares[i,7].RemovePiece();
				PromotionFile = i;
				var scene = GD.Load<PackedScene>("res://WhitePromotion.tscn");
				var controls = (WhitePromotion)scene.Instance();
				var parent = (GameSpace)GetParent();
				float width = parent.BoardWidth() / 8;
				controls.RectPosition = 
					new Vector2(width * i, 0);
				controls.RectScale = new Vector2(0.9F, 0.9F);
				AddChild(controls);
				return;
			}
			else if (squares[i,0].GetPieceName() == "Pawn") {
				Active = false;
				squares[i,0].RemovePiece();
				PromotionFile = i;
				var scene = GD.Load<PackedScene>("res://BlackPromotion.tscn");
				var controls = (BlackPromotion)scene.Instance();
				var parent = (GameSpace)GetParent();
				float width = parent.BoardWidth() / 8;
				controls.RectPosition = 
					new Vector2(width * i, width * 4);
				controls.RectScale = new Vector2(0.9F, 0.9F);
				AddChild(controls);
				return;
			}
		}
	}
	
	private void CheckForEnd() {
		if (AllMoves(squares, Turn).Count == 0) {
			if (LookForChecks(squares, Turn)) 
				Checkmate(Turn == 'w' ? 'b' : 'w');
			else 
				Stalemate(Turn);
		}
	}
	
	private void Checkmate(char col) {
		string winner = (col == 'w' ? "White" : "Black");
		GD.Print($"Checkmate! {winner} wins!");
	}
	
	private void Stalemate(char col) {
		string stuck = (col == 'w' ? "White" : "Black");
		GD.Print($"Stalemate! {stuck} has no legal moves and so the game is a draw.");
	}
	
	private void OnWhitePromotion(string pieceName) {
		squares[PromotionFile,7].RemovePiece();
		squares[PromotionFile,7].BestowPiece(pieceName, 'w');
		var menu = (VBoxContainer)GetChildren()[64];
		((Node)GetChildren()[64]).QueueFree();
		Active = true;
	}
	
	private void OnBlackPromotion(string pieceName) {
		squares[PromotionFile,0].RemovePiece();
		squares[PromotionFile,0].BestowPiece(pieceName, 'b');
		var menu = (VBoxContainer)GetChildren()[64];
		((Node)GetChildren()[64]).QueueFree();
		Active = true;
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
