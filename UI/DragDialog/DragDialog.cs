using Godot;
using System;

public partial class DragDialog : Control {
	[Signal] public delegate void DragDialogCloseEventHandler(int result);

	private LineEdit _result;
	
	public override void _Ready() {
		GlobalPosition = GetGlobalMousePosition();
		GlobalPosition = new Vector2(GlobalPosition.X - Size.X / 2, GlobalPosition.Y - Size.Y / 2);
		_result = GetNode<LineEdit>("Panel/VFlowContainer/LineEdit");
		_result.CallDeferred("grab_focus");
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventKey {Pressed: true, PhysicalKeycode: Key.Enter or Key.KpEnter}) {
			int result;
			if (!int.TryParse(_result.Text, out result)) 
				result = 0;
			
			EmitSignal(SignalName.DragDialogClose, result);
			QueueFree();		 
		}

		if (@event is InputEventKey {Pressed: true, PhysicalKeycode: Key.Escape}) {
			GD.Print("free");
			QueueFree();
		}
	}
}
