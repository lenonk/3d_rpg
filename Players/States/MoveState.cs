using Godot;
using System;

public partial class MoveState : State 
{
	public override void _Ready() {
		base._Ready();
		_animation.Travel("Walk");
	}

	public override void _PhysicsProcess(double delta) {
		Vector3 velocity = _player.Velocity;
		if (Input.IsActionJustPressed("Jump") && _player.IsOnFloor()) {
			//velocity.X = Mathf.MoveToward(_player.Velocity.X, 0, _player.Speed);
			//velocity.Z = Mathf.MoveToward(_player.Velocity.Z, 0, _player.Speed);
			//_player.Velocity = velocity;
			_player.ChangeState("Jump");
		}
		else if (Input.IsActionJustPressed("Attack") && _player.IsOnFloor()) {
			//velocity.X = Mathf.MoveToward(_player.Velocity.X, 0, _player.Speed);
			//velocity.Z = Mathf.MoveToward(_player.Velocity.Z, 0, _player.Speed);
			//_player.Velocity = velocity;
			_player.ChangeState("Attack");
		}
		else if (_player.Direction != Vector3.Zero) {
			Vector3 rotAngle = _player.Mesh.Rotation; 
			rotAngle.Y = (float)Mathf.LerpAngle(_player.Mesh.Rotation.Y,
				Mathf.Atan2(_player.Direction.X, _player.Direction.Z) - _player.Rotation.Y, delta * 10);
			_player.Mesh.Rotation = rotAngle;
			velocity.X = _player.Direction.X * _player.Speed;
			velocity.Z = _player.Direction.Z * _player.Speed;
			_player.Velocity = velocity;
		}
		else {
			_player.ChangeState("Idle");
		}
	}
	
	public override void Exit() {
		GD.Print("Exiting MoveState");
	}
}
