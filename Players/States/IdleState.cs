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
			
		Vector3 velocity = _player.Velocity;
		velocity.X = Mathf.MoveToward(_player.Velocity.X, 0, _player.Speed);
		velocity.Z = Mathf.MoveToward(_player.Velocity.Z, 0, _player.Speed);
		
		_player.Velocity = velocity;
	}
	
	public override void Exit() {
		GD.Print("Exiting IdleState");
	}
}
