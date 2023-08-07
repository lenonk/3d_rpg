using Godot;
using System;
using Playback = Godot.AnimationNodeStateMachinePlayback;

public abstract partial class State : Node {
	protected AnimationTree _animTree;
	protected Playback _animation;
	protected Node3D _parent;
	protected Player _player;

	public override void _Ready() {
		_player = (Player)GetParent();
		_animTree = _parent?.GetNode<AnimationTree>("AnimationTree");
	}
	
	public void Setup(Playback animation, Node3D parent) {
		_animation = animation;
		_parent = parent;
	}

	public abstract void Process(double delta);
	public abstract void Exit();
}
