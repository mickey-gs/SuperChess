using Godot;
using System;
using System.IO;
using System.Diagnostics;

public class Stockfish : Node {
	public override void _Ready() {
		using (Process engine = new Process()) {
			engine.StartInfo.FileName = "./stockfish/stockfish.exe";
			engine.StartInfo.UseShellExecute = false;
			engine.StartInfo.RedirectStandardInput = true;
			engine.StartInfo.RedirectStandardOutput = true;
			
			engine.Start();
			
			StreamWriter streamWriter = engine.StandardInput;
			StreamReader streamReader = engine.StandardOutput;
			
			streamWriter.WriteLine("isready\n");
			streamReader.ReadLine();
			streamReader.ReadLine();
			streamWriter.WriteLine("go\n");
			string output = null;
			for (int i = 0; i < 50; i++) {
				if (i == 48) 
					streamWriter.WriteLine("stop\n");
				
				output = streamReader.ReadLine();
			}
			GD.Print(output);
		}
	}
}
