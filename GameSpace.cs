using Godot;
using System;

public class GameSpace : Area2D
{
	private string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 1 0";
	private Timer[] Timers = new Timer[2];
	private float[] Times = new float[2];

	public override void _Ready() {
		var scene = GD.Load<PackedScene>("res://Board.tscn");
		Board board = (Board)scene.Instance();
		board = FENParser.Parse(Fen);
		AddChild(board);
		board.Connect("TurnChange", this, nameof(_on_Turn_Change));
		
		var bTimer = (Timer)(GetNode("BClock").GetNode("Timer"));
		bTimer.Connect("timeout", this, nameof(_on_Black_Timeout));

		var wTimer = (Timer)(GetNode("WClock").GetNode("Timer"));
		wTimer.Connect("timeout", this, nameof(_on_White_Timeout));
	
		Timers = new Timer[2] {wTimer, bTimer};
		Times = new float[2] {wTimer.TimeLeft, bTimer.TimeLeft};
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
	
	private void StopClocks() {
		foreach (var timer in Timers)
			timer.Stop();
	}
	
	public void _on_White_Timeout() {
		((Board)GetChildren()[4]).Timeout('w');
		StopClocks();
	}
	
	public void _on_Black_Timeout() {
		((Board)GetChildren()[4]).Timeout('b');
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
}
