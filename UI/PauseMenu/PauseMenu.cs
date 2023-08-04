using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	public override void _UnhandledInput(InputEvent @event) {
		if (@event is not InputEventKey {Pressed: true, PhysicalKeycode: Key.Escape})
			return;

		GetTree().Paused = !GetTree().Paused;
		Visible = !Visible;
	}
}
