using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	public override void _Ready() {
		foreach (var node in GetTree().GetNodesInGroup("Players")) {
			if (node is not Player p) return;
			p.PauseMenuSignal += ActivatePauseMenu;
		}
	}

	private void ActivatePauseMenu(Player p) {
		Visible = true;
		GetTree().Paused = true;
	}

	public override void _UnhandledKeyInput(InputEvent @event) {
		if (@event is not InputEventKey {Pressed: true, PhysicalKeycode: Key.Escape})
			return;
		
		GetTree().Paused = false;
		Visible = false;
		GetViewport().SetInputAsHandled();
	}
}
