using Godot;
using System;
using System.Collections.Generic;

public class Board : Node2D
{
	[Export]
	public Color LightSquares = new Color(1,1,1,(float)0.5);
	
	[Signal]
	public delegate void TurnChange();
	[Signal]
	public delegate void GameEnd();
	
	public Vector2 ScreenSize;
	private Square[,] squares;
	public Vector2 EnPassantSq = new Vector2(-1, -1);
	private char Turn = 'w';
	private Square Selected = null;
	private string Fen = StandardFEN;
	private int PromotionFile = 9;
	private bool Active = true;
	private bool[] CastleRights = {true, true, true, true};
	private int[] Halfmoves = {1, 0};

	public const string StandardFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 1 0";
	
	public Board() {
		squares = new Square[8,8];
		var scene = GD.Load<PackedScene>("res://Square.tscn");
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				squares[i,j] = (Square)scene.Instance();
				squares[i,j].SetPos(new Vector2(i, j));
			}
		}
	}
	
	public Board(Square[,] board) {
		squares = new Square[8,8];
		var scene = GD.Load<PackedScene>("res://Square.tscn");
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				squares[i,j] = (Square)scene.Instance();
				squares[i,j].SetPos(new Vector2(i, j));
				if (board[i,j].GetPieceColour() != 'n') {
					squares[i,j].BestowPiece(board[i,j].GetPieceName(), board[i,j].GetPieceColour());
				}
			}
		}
	}
	
	public Board(Square[,] board, char turn, bool[] rights, Vector2 enPSq, int[] halfmoves) {
		Turn = turn;
		CastleRights = rights;
		EnPassantSq = enPSq;
		Halfmoves = halfmoves;
		squares = new Square[8,8];
		var scene = GD.Load<PackedScene>("res://Square.tscn");
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				squares[i,j] = (Square)scene.Instance();
				squares[i,j].SetPos(new Vector2(i, j));
				if (board[i,j].GetPieceColour() != 'n') {
					squares[i,j].BestowPiece(board[i,j].GetPieceName(), board[i,j].GetPieceColour());
				}
			}
		}
	}

	public override void _Ready() {
		var parent = (GameSpace)GetParent();
		float width = parent.BoardWidth();
		float yOffset = parent.BoardOffset().y;
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				AddChild(squares[i,j]);
				squares[j,i].SetPos(new Vector2(j, i));
				squares[j,i].Position = 
					new Vector2((width / 8) * j, yOffset + (width - (width / 8) * (i + 1)));
			}
		}
	}
	
	public List<Vector2> AllAttacks(char col) {
		List<Vector2> allAttacks = new List<Vector2> {};
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (squares[i, j].GetPieceColour() == col) {
					try {
						var p = (Piece)squares[i, j].GetChildren()[3];
						allAttacks.AddRange(p.Moves(squares, squares[i, j].Pos, EnPassantSq));
					}
					catch (System.IndexOutOfRangeException) {}
				}
			}
		}
		
		return allAttacks;
	}
	
	public void LoadFrom(Board toLoad) {
		Turn = toLoad.Turn;
		Halfmoves = toLoad.Halfmoves;
		squares = toLoad.squares;
		EnPassantSq = toLoad.EnPassantSq;
		CastleRights = toLoad.CastleRights;
		_Ready();
	}
	
	private List<Vector2> AllMoves(Square[,] board, char col) {
		List<Vector2> allMoves = new List<Vector2> {};
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (board[i, j].GetPieceColour() == col) {
					try {
						var p = (Piece)board[i, j].GetChildren()[3];
						allMoves.AddRange(p.LegalMoves(board, board[i, j].Pos, this));
					}
					catch (System.IndexOutOfRangeException) {}
				}
			}
		}
		
		return allMoves;
	}
	
	private bool LookForMate(char col) {
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (squares[i, j].GetPieceColour() == col) {
					try {
						var p = (Piece)squares[i, j].GetChildren()[3];
						if (p.LegalMoves(squares, squares[i, j].Pos, this).Count != 0)
							return false;
					}
					catch (System.IndexOutOfRangeException) {
						continue;
					}
				}
			}
		}
		
		return true;
	}
	
	public bool LookForChecks(char col) {
		try {
		Square kingSquare = null;
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (squares[i,j].GetPieceName() == "King" && squares[i,j].GetPieceColour() == col)
					kingSquare = squares[i,j];
			}
		}
		char enemyCol = (col == 'w' ? 'b' : 'w');
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (squares[i,j].GetPieceColour() != 'n' && 
					((Piece)squares[i,j].GetChildren()[3]).Moves(squares, squares[i,j].Pos, EnPassantSq).Contains(kingSquare.Pos))
					return true;
			}
		}
		}
		catch (System.NullReferenceException) {}
		return false;
	}
	
	public bool CastleKingside(Piece king) {
		if (LookForChecks(king.Colour)) 
			return false;

		if (king.Colour == 'w' && CastleRights[1]) {
			for (int i = 5; i < 7; i++) {
				if (squares[i,0].GetPieceColour() != 'n'|| 
					AllAttacks('b').Contains(new Vector2(i,0))) {
					return false;
				}
			}
			return true;
		}
		else if (CastleRights[3]) {
			for (int i = 5; i < 7; i++) {
				if (squares[i,7].GetPieceColour() != 'n'|| 
					AllAttacks('w').Contains(new Vector2(i,7)))
					return false;
			}
			return true;
		}
		return false;
	}
	
	public bool CastleQueenside(Piece king) {
		if (LookForChecks(king.Colour)) 
			return false;
		if (king.Colour == 'w' && CastleRights[0]) {
			for (int i = 3; i >= 1; i--) {
				if (squares[i,0].GetPieceColour() != 'n' || 
					AllAttacks('b').Contains(new Vector2(i,0))) {
					return false;
				}
			}
			return true;
		}
		else if (CastleRights[2]) {
			for (int i = 3; i >= 1; i--) {
				if (squares[i,7].GetPieceColour() != 'n'|| 
					AllAttacks('w').Contains(new Vector2(i,7)))
					return false;
			}
			return true;
		}
		return false;
	}
	
	public void Timeout(char col) {
		if (col == 'b') {
			GD.Print("White wins on time.");
			Active = false;
		}
		else if (col == 'w') {
			GD.Print("Black wins on time.");
			Active = false;
		}
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
		
		var parent = (GameSpace)GetParent();
		float offset = parent.BoardOffset().y;
		float width = parent.BoardWidth();
		int file = (int)((position.x / width) * 8);
		int rank = (int)((((offset + width) - position.y) / width) * 8);
			
		try {	
			if (Selected == null) {
				if (squares[file, rank].GetPieceColour() != Turn)
					return;
				
				Selected = squares[file, rank];
				var p = (Piece)squares[file, rank].GetChildren()[3];
				squares[file, rank].Highlight();
				foreach (var dest in p.LegalMoves(squares, new Vector2(file, rank), this)) {
					squares[(int)dest.x, (int)dest.y].Highlight();
				}
			}
			else {
				if (squares[file, rank].GetPieceColour() == Turn) {
					HighlightMoves(file, rank);
				}
				else { 
					var p = (Piece)Selected.GetChildren()[3];
					if (p.LegalMoves(squares, Selected.Pos, this).Contains(new Vector2(file, rank))) {
						HandleMove(file, rank, p);
						CheckForEnd();
						LookForPromotion();
					}
					else {
						Selected = null;
					}
				}
			}
		}
		catch (System.IndexOutOfRangeException) {}
	}
	
	private void HighlightMoves(int file, int rank) {
		Selected = squares[file, rank];
		var p = (Piece)squares[file, rank].GetChildren()[3];
		squares[file, rank].Highlight();
		foreach (var dest in p.LegalMoves(squares, new Vector2(file, rank), this)) {
			squares[(int)dest.x, (int)dest.y].Highlight();
		}
	}
	
	private void RemoveRights(char col) {
		if (col == 'w') {
			CastleRights[0] = false;
			CastleRights[1] = false;
		}
		else {
			CastleRights[2] = false;
			CastleRights[3] = false;
		}
	}
	
	private void RemoveRights(char col, int file) {
		if (col == 'w') {
			CastleRights[file == 0 ? 0 : 1] = false;
		}
		else {
			CastleRights[file == 0 ? 2 : 3] = false;
		}
	}
	
	private void HandleMove(int file, int rank, Piece piece) {
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
		
		if (squares[file, rank].GetPieceName() == "King" && Math.Abs(file - Selected.Pos.x) > 1) {
			int kingRank = (squares[file, rank].GetPieceColour() == 'w' ? 0 : 7);
			if (file == 2) {
				squares[0,kingRank].RemovePiece();
				squares[3,kingRank].BestowPiece("Rook", squares[file,rank].GetPieceColour());
			}
			else {
				squares[7,kingRank].RemovePiece();
				squares[5,kingRank].BestowPiece("Rook", squares[file,rank].GetPieceColour());
			}
		}
		
		if (squares[file,rank].GetPieceName() == "King") 
			RemoveRights(squares[file,rank].GetPieceColour());
			
		if (squares[file,rank].GetPieceName() == "Rook")
			RemoveRights(squares[file,rank].GetPieceColour(), file);
	
		EmitSignal(nameof(TurnChange), Turn);
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
		if (LookForMate(Turn)) {
			Active = false;
			if (LookForChecks(Turn))
				Checkmate(Turn == 'w' ? 'b' : 'w');
			else
				Stalemate(Turn);
		
			EmitSignal(nameof(GameEnd));
		}
	}
	
	private void HighlightLoser(char col) {
		Square kingSquare = null;
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (squares[i,j].GetPieceName() == "King" && squares[i,j].GetPieceColour() == col)
					kingSquare = squares[i,j];
			}
		}
		kingSquare.RedHighlight();
	}
	
	private void Checkmate(char col) {
		string winner = (col == 'w' ? "White" : "Black");
		HighlightLoser(col == 'w' ? 'b' : 'w');
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
