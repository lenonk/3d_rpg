using Godot;
using System;

public partial class Player : Entity
{
	[Export] public float Speed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float Acceleration = 5.5f;

	[Signal] public delegate void PauseMenuEventHandler(Player p);
	
	private AnimationTree _anim;
	private AnimationNodeStateMachinePlayback _playback;
	private State _state = null;
	private Inventory _inventory;
	
	private int _health = 10;

	public Vector3 Direction;
	public Node3D Mesh;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready() {
		base._Ready();
		
		_anim = GetNode<AnimationTree>("AnimationTree");
		_playback = (AnimationNodeStateMachinePlayback)_anim.Get("parameters/playback");
		Mesh = GetNode<Node3D>("Skeleton3D");
		
		ChangeState("Idle");
		// TODO: Remove this.  Testing only!
		BuildInventory();
	}
	
	public override void _PhysicsProcess(double delta) {
		Vector3 velocity = Velocity;
		Direction = Vector3.Zero;
		
		if (!IsOnFloor())
			velocity.Y -= _gravity * (float)delta;
		
		var hRot = GetNode<Node3D>("CameraController").Basis.GetEuler().Y;
		Direction = GetDirection(); 
		Direction = Direction.Rotated(Vector3.Up, hRot).Normalized();

		Velocity = velocity;
		MoveAndSlide();
	}
	
	public override void _UnhandledInput(InputEvent @event) {
		if (@event is not InputEventKey {Pressed: true, PhysicalKeycode: Key.Escape})
			return;

		EmitSignal(SignalName.PauseMenu, this);
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

	public void Stop(double delta, bool now = false) {
		Vector3 velocity = Velocity;
		if (now) {
			velocity.X = 0;
			velocity.Z = 0;
		}
		else {
			velocity.X = (float)Mathf.Lerp(Velocity.X, 0, Acceleration * delta);
			velocity.Z = (float)Mathf.Lerp(Velocity.Z, 0, Acceleration * delta);
		}
		
		Velocity = velocity;
	}
	
	public override void TakeDamage(int value) {
		_health -= value;
		if (_health <= 0) Die();
	}
    	
	private Vector3 GetDirection() {
		return new Vector3(
			Input.GetActionStrength("WalkLeft") - Input.GetActionStrength("WalkRight"),
			0,
			Input.GetActionStrength("WalkUp") - Input.GetActionStrength("WalkDown"));
	}

	private void Die() {
	}

	public State GetState() => _state;

	private void BuildInventory() {
		Inventory.Item i1 = new("Rune Sword", "Badass Sword", 1);
		Inventory.Item i2 = new("Rune Hammer", "Badass Hammer", 1);
		Inventory.Item i3 = new("Rune Staff", "Badass Staff", 1);
		
		_inventory.AddItem(i1);
		_inventory.AddItem(i2);
		_inventory.AddItem(i3);
	}
}
