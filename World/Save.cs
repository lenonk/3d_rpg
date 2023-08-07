using Godot;
using System;

public partial class Save : Node {
	private const string SavePath = "res://savegame.bin";
	private const string SavePass = "password";

	private const string SaveData = "alsdkfjalksdjfasdf";
	
	public void SaveGame() {
		var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
		var jstr = Json.Stringify(SaveData);
		file.StoreLine(jstr);
	}

	public void LoadGame() {
		var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
		if (file == null) return;

		while (!file.EofReached()) {
			var data = Json.ParseString(file.GetLine());
			// Do something with the data
		}
	}
}
