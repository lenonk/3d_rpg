using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float Acceleration = 5.5f;

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
	
	public override void _PhysicsProcess(double delta) {
		Vector3 velocity = Velocity;
		
		if (!IsOnFloor())
			velocity.Y -= (float)(_gravity * delta);
		
		var hRot = GetNode<Node3D>("CameraController").Basis.GetEuler().Y;
		Direction = GetDirection(); 
		Direction = Direction.Rotated(Vector3.Up, hRot).Normalized();

		Velocity = velocity;
		MoveAndSlide();
	}
	
	public void ChangeState(string stateName) {
		_state?.Exit();
		_state?.QueueFree();

		if ((_state = StateFactory.GetInstance(stateName)) != null) {
			_state.Setup(_playback, this);
			_state.Name = stateName;
			AddChild(_state);
		}
	}

	public void Stop(double delta) {
		Vector3 velocity = Velocity;
		velocity.X = (float)Mathf.Lerp(Velocity.X, 0, Acceleration * delta);
		velocity.Z = (float)Mathf.Lerp(Velocity.Z, 0, Acceleration * delta);
		Velocity = velocity;
	}
	
	private Vector3 GetDirection() {
		return new Vector3(
			Input.GetActionStrength("WalkLeft") - Input.GetActionStrength("WalkRight"),
			0,
			Input.GetActionStrength("WalkUp") - Input.GetActionStrength("WalkDown"));
	}
	
	public State GetState() => _state;

}
