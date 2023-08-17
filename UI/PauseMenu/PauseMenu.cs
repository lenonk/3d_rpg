using Godot;
using System;
using System.Linq;

public partial class PauseMenu : CanvasLayer {
	private Player _player;
	
	public override void _Ready() {
		_player = GetTree().GetNodesInGroup("Players")[0] as Player;
	}
	
	private void ShowPauseMenu() {
		Visible = true;
		GetTree().Paused = true;
	}
	
	private void HidePauseMenu() {
		GetTree().Paused = false;
		Visible = false;
	}

	public override void _UnhandledKeyInput(InputEvent @event) {
		switch (@event) {
			case InputEventKey {PhysicalKeycode: Key.Escape, Pressed: true}:
				if (!Visible) { ShowPauseMenu(); }
				else { HidePauseMenu(); }
				GetViewport().SetInputAsHandled();
				break;
			case InputEventKey {Pressed: true}:
				if (Visible)
					GetViewport().SetInputAsHandled();
				break;
		}
	}
}
