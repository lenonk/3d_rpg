using Godot;
using System;

public partial class WalkState : State 
{
	public override void _Ready() {
		base._Ready();
		_animation.Travel("Walk");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta) {
		Vector3 velocity = _player.Velocity;
		if (Input.IsActionJustPressed("Jump") && _player.IsOnFloor()) {
			_player.Stop(delta);
			_player.ChangeState("Jump");
		}
		else if (Input.IsActionJustPressed("Attack") && _player.IsOnFloor()) {
			_player.Stop(delta);
			_player.ChangeState("Attack");
		}
		else if (_player.Direction != Vector3.Zero) {
			_player.Mesh.Rotation = GetMeshRotationAngle(delta);
			_player.Velocity = GetVelocity(velocity, delta);
		}
		else {
			_player.ChangeState("Idle");
		}
	}

	private Vector3 GetVelocity(Vector3 velocity, double delta) {
		velocity.X = (float)Mathf.Lerp(
			velocity.X, 
			_player.Direction.X * _player.Speed, 
			_player.Acceleration * delta);
		velocity.Z = (float)Mathf.Lerp(
			velocity.Z, 
			_player.Direction.Z * _player.Speed, 
			_player.Acceleration * delta);

		return velocity;
	}
	
	private Vector3 GetMeshRotationAngle(double delta) {
			Vector3 angle = _player.Mesh.Rotation; 
			angle.Y = (float)Mathf.LerpAngle(
				_player.Mesh.Rotation.Y,
				Mathf.Atan2(_player.Direction.X, _player.Direction.Z) - _player.Rotation.Y, 
				delta * 10);

			return angle;
	}

	public override void Exit() {
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
