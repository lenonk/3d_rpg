using Godot;
using System;
using System.Threading.Tasks;

public partial class AttackState : State
{
	public override void _Ready() {
		base._Ready();
		_animation.Travel("Attack(1h)");
	}

	public override async void Process(double delta) {
		//_player.Stop(delta, true);
		_animTree.Advance(delta * 0.1f);
		
		if (_animation.IsPlaying() && _animation.GetCurrentNode()== "Attack(1h)") {
			await ToSignal(_animTree, "animation_finished");
			_player.ChangeState("Idle");
		}
	}
	
	public override void Exit() {
	}
}
