using Godot;
using System;

public partial class Player : Entity
{
	[Export] public float Speed = 5.5f;
	[Export] public float JumpVelocity = 15.5f;
	[Export] public float Acceleration = 5.5f;
	[Export] public float FallFactor = 240.5f;

	[Signal] public delegate void PauseMenuSignalEventHandler(Player p);
	
	private AnimationTree _anim;
	private AnimationNodeStateMachinePlayback _playback;
	private State _state = null;
	private Inventory _inventory = new();
	
	private int _health = 10;

	public Vector3 Direction;
	public Node3D Mesh;
	
	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	public override void _Ready() {
		base._Ready();
		
		_anim = GetNode<AnimationTree>("AnimationTree");
		_playback = (AnimationNodeStateMachinePlayback)_anim.Get("parameters/playback");
		Mesh = GetNode<Node3D>("Skeleton3D");
		
		AddToGroup("Players");
		ChangeState("Idle");
		
		// TODO: Remove this.  Testing only!
		BuildInventory();
		RenderingServer.SetDefaultClearColor(Colors.DodgerBlue);
	}
	
	public override void _PhysicsProcess(double delta) {
		Direction = Vector3.Zero;
		
		var hRot = GetNode<Node3D>("CameraController").Basis.GetEuler().Y;
		Direction = GetDirection(); 
		Direction = Direction.Rotated(Vector3.Up, hRot).Normalized();

		_state?.Process(delta);

		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y -= (float)(_gravity * FallFactor * delta * delta);

		Velocity = velocity;
		MoveAndSlide();
	}
	
	public override void _UnhandledKeyInput(InputEvent @event) {
		if (@event is not InputEventKey {Pressed: true, PhysicalKeycode: Key.Escape})
			return;

		EmitSignal(SignalName.PauseMenuSignal, this);
		GetViewport().SetInputAsHandled();
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
	public Inventory GetInventory() => _inventory;
	
	private void BuildInventory() {
		_inventory.AddItem(Items.CreateItem("Wooden Sword"));
		_inventory.AddItem(Items.CreateItem("White Sword"));
		_inventory.AddItem(Items.CreateItem("Magical Sword"));
	}
}
