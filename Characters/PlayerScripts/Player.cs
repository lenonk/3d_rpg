using Godot;
using Godot.Collections;
using System;
using System.Linq;
using static Items;

public partial class Player : Entity
{
	[Export] public float Speed = 5.5f;
	[Export] public float JumpVelocity = 15.5f;
	[Export] public float Acceleration = 5.5f;
	[Export] public float FallFactor = 240.5f;
	[Export] public float MaxLockonDistance = 15.0f;

	[Signal] public delegate void PauseMenuSignalEventHandler(Player p);
	
	private AnimationTree _anim;
	private AnimationNodeStateMachinePlayback _playback;
	private State _state = null;
	private Inventory _inventory = new();
	private Entity _lockedTarget;
	
	public Vector3 Direction;
	public Node3D Mesh;
	
	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	public override void _Ready() {
		base._Ready();
		
		_anim = GetNode<AnimationTree>("AnimationTree");
		_playback = (AnimationNodeStateMachinePlayback)_anim.Get("parameters/playback");
		Mesh = GetNode<Node3D>("Skeleton3D");
		Health = MaxHealth = 50;
		
		AddToGroup("Players");
		SetupSignals();
		ChangeState("Idle");
		
		// TODO: Remove this.  Testing only!
		BuildInventory();
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
		switch (@event) {
			case InputEventKey {PhysicalKeycode: Key.Escape, Pressed: true}:
				EmitSignal(SignalName.PauseMenuSignal, this);
				GetViewport().SetInputAsHandled();
				break;
			case InputEventKey {PhysicalKeycode: Key.E, Pressed: true}:
				var enemies = GetTree().GetNodesInGroup("Enemies");

				Entity closest = null;
				foreach (Entity enemy in enemies) {
					if (IsValidTarget(enemy) && DistanceTo(enemy) < DistanceTo(closest))
						closest = enemy;
				}
				_lockedTarget = closest;
				break;
		}
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
	
	private Vector3 GetDirection() {
		return new Vector3(
			Input.GetActionStrength("WalkLeft") - Input.GetActionStrength("WalkRight"),
			0,
			Input.GetActionStrength("WalkUp") - Input.GetActionStrength("WalkDown"));
	}

	public override void Die() {
	}

	private void ChangeEquipment(BoneAttachment3D slot, Item item, bool equip) {
		var _eqScene = ResourceLoader.Load<PackedScene>(item.Scene);
		if (equip) {
			if (_eqScene.Instantiate() is not EquipmentBase { } eq) return;
			eq.Name = item.Name;
			slot.AddChild(eq);
		}
		else {
			foreach (var node in slot.GetChildren()) {
				if (node.Name == item.Name) {
					node.Free();
				}
			}
		}

		if (_inventory.GetItem(item.Name, out var i))
			i.IsWearing = equip;
	}
	
	private void OnEquipmentChanged(Item item, bool equip) {
		BoneAttachment3D slot = null;
		switch (item.Type) {
			case ItemType.Weapon:
				slot = GetNode<BoneAttachment3D>("Skeleton3D/HandSlotRight");
				break;
			case ItemType.Body:
				slot = GetNode<BoneAttachment3D>("Skeleton3D/Body");
				break;
			case ItemType.Head:
				slot = GetNode<BoneAttachment3D>("Skeleton3D/Head");
				break;
			case ItemType.Shield:
				slot = GetNode<BoneAttachment3D>("Skeleton3D/HandSlotLeft");
				break;
			case ItemType.Waist:
				break;
			case ItemType.Misc:
				break;
		}
		
		if (slot is not null)
			ChangeEquipment(slot, item, equip);
	}
	
	private void SetupSignals() {
		var pm = GetTree().Root.GetNode<PauseMenu>("World/PauseMenu");
		pm.Connect(PauseMenu.SignalName.EquipmentChangedSignal, 
			new Callable(this, nameof(OnEquipmentChanged)));
	}
	
	
	public State GetState() => _state;
	public Inventory GetInventory() => _inventory;
	private float DistanceTo(Node3D node) => node != null ? GlobalPosition.DistanceTo(node.GlobalPosition) : Mathf.Inf;
	public bool IsValidTarget(Node node) => node is Entity enemy && DistanceTo(enemy) < MaxLockonDistance && enemy != _lockedTarget;
	
	private void BuildInventory() {
		_inventory.AddItem(CreateItem("Iron Dagger"));
		_inventory.AddItem(CreateItem("Iron Double Axe"));
		_inventory.AddItem(CreateItem("Archer's Hood"));
		_inventory.AddItem(CreateItem("Archer's Bandolier"));
		_inventory.AddItem(CreateItem("Wooden Shield"));
		_inventory.AddItem(CreateItem("Magical Sword"));
		_inventory.AddItem(CreateItem("Magical Sword"));
	}
}
