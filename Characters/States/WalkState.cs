using Godot;
using System;

public partial class WalkState : State 
{
	public override void _Ready() {
		base._Ready();
		_animation.Travel("Walk");
	}

	public override void Process(double delta) {
		Vector3 velocity = _player.Velocity;
		
		if (Input.IsActionJustPressed("Jump") && _player.IsOnFloor()) {
			_player.ChangeState("Jump");
		}
		else if (Input.IsActionPressed("Attack") && _player.IsOnFloor()) {
			_player.ChangeState("Attack");
		}
		else if (_player.Direction != Vector3.Zero) {
			_player.Mesh.Rotation = GetMeshRotationAngle(delta);
			GetVelocity(ref velocity, delta);
		}
		else {
			_player.ChangeState("Idle");
		}

		_player.Velocity = velocity;
	}

	private void GetVelocity(ref Vector3 velocity, double delta) {
		velocity.X = (float)Mathf.Lerp(
			velocity.X, 
			_player.Direction.X * _player.Speed, 
			_player.Acceleration * delta);
		velocity.Z = (float)Mathf.Lerp(
			velocity.Z, 
			_player.Direction.Z * _player.Speed, 
			_player.Acceleration * delta);
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
	}
}
