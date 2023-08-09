using Godot;
using System;

public partial class DragPreview : Sprite2D {
	public override void _Process(double delta) {
		GlobalPosition = GetGlobalMousePosition();
		
		if (Input.IsActionJustReleased("ui_left_mouse"))
			QueueFree();
	}
}
