using Godot;
using System;

public partial class JumpState : State {
	
	public override void _Ready() {
		base._Ready();
		_animation.Travel("Jump");
	}
	
	public override async void Process(double delta) {
		Vector3 velocity = _player.Velocity;

		if (_player.IsOnFloor()) {
			await ToSignal(GetTree().CreateTimer(0.4f), "timeout");
			velocity.Y = _player.JumpVelocity;
		}
	
		if (_player.IsOnFloor()) {
			_player.ChangeState("Idle");
		}

		_player.Velocity = velocity;
	}

	
	public override void Exit() {
	}
}
