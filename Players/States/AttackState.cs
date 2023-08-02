using Godot;
using System;

public partial class AttackState : State
{
	public override void _Ready() {
		base._Ready();
		_animation.Travel("Attack(1h)");
	}

	public override async void _PhysicsProcess(double delta) {
		if (_animation.IsPlaying() && _animation.GetCurrentNode()== "Attack(1h)") {
			await ToSignal(_animTree, "animation_finished");
			_player.ChangeState("Idle");
		}
	}
	
	public override void Exit() {
		GD.Print("Exiting AttackState");
	}
}
