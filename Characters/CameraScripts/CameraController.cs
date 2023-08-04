using Godot;
using System;

public partial class CameraController : Node3D {
	private const float Sensitivity = 5;

	private SpringArm3D _springArm;
	
	public override void _Ready() {
		_springArm = GetNode<SpringArm3D>("SpringArm3D");
		_springArm.SpringLength = 4;
	}
	
	public override void _PhysicsProcess(double delta) {
		if (GetParent<Node3D>() is not Player) return;
		
		GlobalPosition = GetNode<Player>("..").GlobalPosition;
	}
	
	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion mEvent && Input.MouseMode == Input.MouseModeEnum.Captured) {
			float xRot = (float)Mathf.Clamp(Rotation.X + mEvent.Relative.Y / 1000 * Sensitivity, -0.25, 0.4);
			float yRot = Rotation.Y - mEvent.Relative.X / 1000 * Sensitivity;
			Rotation = new Vector3(xRot, yRot, 0);
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
			else if (button.ButtonIndex == MouseButton.Middle) {
				if (button.Pressed)
					Input.MouseMode = Input.MouseModeEnum.Captured;
				else
					// TODO:  Only if player state is not walking
					Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}
	}
}
