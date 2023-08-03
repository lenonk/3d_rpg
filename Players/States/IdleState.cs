using Godot;
using System;

public partial class IdleState : State
{
	public override void _Ready() {
		base._Ready();
		_animation.Travel("Idle");
	}	
	
	public override void _PhysicsProcess(double delta) {
		if (Input.IsActionPressed("WalkRight") ||
        		    Input.IsActionPressed("WalkLeft") ||
        		    Input.IsActionPressed("WalkUp") ||
        		    Input.IsActionPressed("WalkDown")) {
        			_player.ChangeState("Walk");
		}

		if (Input.IsActionJustPressed("Jump") && _player.IsOnFloor())
			_player.ChangeState("Jump");
		if (Input.IsActionJustPressed("Attack") && _player.IsOnFloor())
			_player.ChangeState("Attack");

		_player.Stop(delta);
	}
	
	public override void Exit() {
	}
}
