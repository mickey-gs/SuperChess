using Godot;
using System;

public class GameSpace : Area2D
{
	private string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 1 0";
	private Timer[] Timers = new Timer[2];
	private float[] Times = new float[2];
	private float[] MaxTimes = new float[2];

	public override void _Ready() {
		var scene = GD.Load<PackedScene>("res://Board.tscn");
		Board board = (Board)scene.Instance();
		board = new Board();
		board.Name = "Board";
		AddChild(board);
		board.Connect("TurnChange", this, nameof(_on_Turn_Change));
		
		var bTimer = (Timer)(GetNode("BClock").GetNode("Timer"));
		bTimer.Connect("timeout", this, nameof(_on_Black_Timeout));

		var wTimer = (Timer)(GetNode("WClock").GetNode("Timer"));
		wTimer.Connect("timeout", this, nameof(_on_White_Timeout));
	
		Timers = new Timer[2] {wTimer, bTimer};
		Times = new float[2] {wTimer.TimeLeft, bTimer.TimeLeft};
		MaxTimes = new float[2] {wTimer.TimeLeft, bTimer.TimeLeft};
		Timers[1].Stop();
		Timers[0].Stop();
	}
	
	public float BoardWidth() {
		var rectangle = (CollisionShape2D)GetNode("CollisionShape2D");
		var shape = (RectangleShape2D)rectangle.Shape;
		return shape.GetExtents().x * 2;
	}
	
	public Vector2 BoardOffset() {
		var rectangle = (CollisionShape2D)GetNode("CollisionShape2D");
		Vector2 ret = new Vector2(-1, -1);
		ret.y = rectangle.Position.y - BoardWidth() / 2;
		return ret;
	}
	
	private void UpdateClocks() {
		for (int i = 0; i < 2; i++) {
			Timers[i].Start(Times[i]);
		}
		
		((ChessClock)GetNode("BClock")).Update();
		((ChessClock)GetNode("WClock")).Update();
		
		Timers[0].Stop();
		Timers[1].Stop();
	}
	
	private void StopClocks() {
		foreach (var timer in Timers)
			timer.Stop();
	}
	
	private void ResetClocks() {
		Times[0] = MaxTimes[0];
		Times[1] = MaxTimes[1];
	
		UpdateClocks();
		StopClocks();
	}
	
	public void _on_Board_GameEnd() {
		StopClocks();
	}
	
	public void _on_White_Timeout() {
		((Board)GetNode("Board")).Timeout('w');
		StopClocks();
	}
	
	public void _on_Black_Timeout() {
		((Board)GetNode("Board")).Timeout('b');
		StopClocks();
	}
	
	public void _on_Turn_Change(char turn) {
		if (turn == 'b') {
			Times[0] = Timers[0].TimeLeft;
			Timers[0].Stop();
			Timers[1].Start(Times[1]);
		}
		else {
			Times[1] = Timers[1].TimeLeft;
			Timers[1].Stop();
			Timers[0].Start(Times[0]);
		}
	}

	private void _on_StartButton_pressed(LineEdit a) {
		var scene = GD.Load<PackedScene>("res://Board.tscn");
		Board board = (Board)scene.Instance();
		board = FENParser.Parse(a.Text);
		board.Name = "Board";
		var b = (Board)GetNode("Board");
		b.LoadFrom(board);
		board.Connect("TurnChange", this, nameof(_on_Turn_Change));
		board.Connect("GameEnd", this, nameof(_on_Board_GameEnd));
		ResetClocks();
	}
	
	
	private void _on_WhiteClockOption_item_selected(int index) {
		switch (index) {
			case 0:
				MaxTimes[0] = 1 * 60;
				break;
			
			case 1:
				MaxTimes[0] = 2 * 60;
				break;
				
			case 2:
				MaxTimes[0] = 3 * 60;
				break;
				
			case 3:
				MaxTimes[0] = 5 * 60;
				break;
				
			case 4:
				MaxTimes[0] = 10 * 60;
				break;
				
			case 5:
				MaxTimes[0] = 15 * 60;
				break;
				
			case 6:
				MaxTimes[0] = 60 * 60;
				break;
		}
		ResetClocks();
	}

	private void _on_BlackClockOption_item_selected(int index) {
		switch (index) {
			case 0:
				MaxTimes[1] = 1 * 60;
				break;
			
			case 1:
				MaxTimes[1] = 2 * 60;
				break;
				
			case 2:
				MaxTimes[1] = 3 * 60;
				break;
				
			case 3:
				MaxTimes[1] = 5 * 60;
				break;
				
			case 4:
				MaxTimes[1] = 10 * 60;
				break;
				
			case 5:
				MaxTimes[1] = 15 * 60;
				break;
				
			case 6:
				MaxTimes[1] = 60 * 60;
				break;
		}
		ResetClocks();
	}
	
	private void OnPromotionMenuAppearing() {
		StopClocks();
	}
}
