using Godot;
using System;

public class ChessClock : RichTextLabel {
	public override void _Ready() {
		if (!Stopped()) {
			int minutes = GetTime() / 60;
			int seconds = GetTime() % 60;
			if (seconds < 10) 
				BbcodeText = $"[center]{minutes}:0{seconds}[/center]";
			else
				BbcodeText = $"[center]{minutes}:{seconds}[/center]";
		}
	}

	public override void _Process(float delta) {
		if (!Stopped()) {
			int minutes = GetTime() / 60;
			int seconds = GetTime() % 60;
			if (seconds < 10) 
				BbcodeText = $"[center]{minutes}:0{seconds}[/center]";
			else
				BbcodeText = $"[center]{minutes}:{seconds}[/center]";
		}
	}
	
	public void Update() {
		int minutes = GetTime() / 60;
		int seconds = GetTime() % 60;
		if (seconds < 10) 
			BbcodeText = $"[center]{minutes}:0{seconds}[/center]";
		else
			BbcodeText = $"[center]{minutes}:{seconds}[/center]";
	}
	
	private int GetTime() {
		return (int)((Timer)GetNode("Timer")).TimeLeft;
	}
	
	private bool Stopped() {
		return ((Timer)GetNode("Timer")).IsStopped();
	}
}
