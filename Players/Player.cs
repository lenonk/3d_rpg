using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;

	private AnimationTree _anim;
	private AnimationNodeStateMachinePlayback _playback;
	private State _state = null;

	public Vector3 Direction;
	public Node3D Mesh;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready() {
		_anim = GetNode<AnimationTree>("AnimationTree");
		_playback = (AnimationNodeStateMachinePlayback)_anim.Get("parameters/playback");
		Mesh = GetNode<Node3D>("Skeleton3D");
		ChangeState("Idle");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		if (!IsOnFloor())
			velocity.Y -= (float)(_gravity * delta);
		Velocity = velocity;
		
		var hRot = GetNode<Node3D>("CameraController").Basis.GetEuler().Y;
		Direction = new Vector3(
			Input.GetActionStrength("WalkLeft") - Input.GetActionStrength("WalkRight"),
			0,
			Input.GetActionStrength("WalkUp") - Input.GetActionStrength("WalkDown"));
		Direction = Direction.Rotated(Vector3.Up, hRot).Normalized();

		MoveAndSlide();
	}

	private State GetInstance(StateFactory.State stateName) {
		Type t;
		if ((t = Type.GetType(stateName.ToString())) != null)
			return (State)Activator.CreateInstance(t);

		return null;
	}
	
	public void ChangeState(string stateName) {
		if (!StateFactory.HasState(stateName)) return;

		_state?.Exit();
		_state?.QueueFree();

		if ((_state = GetInstance(StateFactory.GetSate(stateName))) == null)
			return;
		
		_state.Setup("ChangeState", _playback, this);
		_state.Name = stateName;
		AddChild(_state);
	}
}
