using Godot;
using static StateFactory;
using System;

public partial class CameraController : Node3D {
	private const float Sensitivity = 5;

	private SpringArm3D _springArm;
	private Player _player;
	private bool _rightButtonPressed = false;
	
	public override void _Ready() {
		_springArm = GetNode<SpringArm3D>("SpringArm3D");
		_springArm.SpringLength = 4;
		if (GetParent<Node3D>() is Player p)
			_player = p;
	}
	
	public override void _PhysicsProcess(double delta) {
		if (_player is null) return;
		
		GlobalPosition = _player.GlobalPosition;
	}
	
	public override void _Input(InputEvent @event) {
		if (_player is null) return;
		
		if (@event is InputEventMouseMotion mEvent && _rightButtonPressed) {
			float xRot = (float)Mathf.Clamp(Rotation.X + mEvent.Relative.Y / 1000 * Sensitivity, -0.25, 0.6);
			float yRot = Rotation.Y - mEvent.Relative.X / 1000 * Sensitivity;
			Rotation = new Vector3(xRot, yRot, 0);
			GetViewport().SetInputAsHandled();
		}

		if (@event is InputEventMouseButton button) {
			if (button.ButtonIndex == MouseButton.WheelDown) {
				if (_springArm.SpringLength < 8)
					_springArm.SpringLength += 0.1f;
			}
			else if (button.ButtonIndex == MouseButton.WheelUp) {
				if (_springArm.SpringLength > 3)
					_springArm.SpringLength -= 0.1f;
			}
			else if (button.ButtonIndex == MouseButton.Right) {
				_rightButtonPressed = button.Pressed;
			}
		}
	}
}
