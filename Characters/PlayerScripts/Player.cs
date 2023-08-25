using Godot;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using static Items;

public partial class Player : Entity
{
	[Export] public float Speed = 5.5f;
	[Export] public float JumpVelocity = 15.5f;
	[Export] public float Acceleration = 5.5f;
	[Export] public float FallFactor = 240.5f;
	[Export] public float MaxLockonDistance = 15.0f;

	private AnimationTree _anim;
	private AnimationNodeStateMachinePlayback _playback;
	private State _state = null;
	private Entity _lockedTarget;
	private Node3D _cameraController;
	
	public Inventory Inventory = new();
	public Equipment Equipment = new();
	public Vector3 Direction;
	public Node3D Mesh;
	
	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	public override void _Ready() {
		base._Ready();
		
		_anim = GetNode<AnimationTree>("AnimationTree");
		_playback = (AnimationNodeStateMachinePlayback)_anim.Get("parameters/playback");
		_cameraController = GetNode<Node3D>("CameraController");
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

		Direction = GetDirection();
		var hRot = GetNode<Node3D>("CameraController").Basis.GetEuler().Y;
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
			case InputEventKey {PhysicalKeycode: Key.Tab, Pressed: true}:
				var enemies = GetTree().GetNodesInGroup("Enemies");
				var sEnemies = enemies.OrderBy(enemy => DistanceTo((Node3D)enemy)).ToList();

				if (!sEnemies.Contains(_lockedTarget)) {
					UnlockTarget();
				}
					
				if (_lockedTarget == sEnemies[^1]) {
					UnlockTarget();
					return;
				}

				// TODO: Change this to just select the next node in sEnemies
				foreach (var node in sEnemies) {
					if (node is not Entity { } enemy) continue;
					if (!IsValidTarget(enemy)) continue; 
					LockTarget(enemy);
					break;
				}

				break;
		}
	}

	private void LockTarget(Entity target) {
		GD.Print($"Locking target {target.Name}");
		_lockedTarget = target;
		_lockedTarget.EntityDied += OnTargetDeath;
	}
	
	private void UnlockTarget() {
		GD.Print($"Unlocking target {_lockedTarget.Name}");
		_lockedTarget.EntityDied -= OnTargetDeath;
		_lockedTarget = null;
	}
		
	private void OnTargetDeath() {
		UnlockTarget();
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

		item.IsWearing = equip;
	}
	
	public void OnEquipmentChanged(int idx, bool equip, int rIdx = -1) {
		BoneAttachment3D slot = null;
		var item = (equip) ? Inventory.At(idx) : Equipment.At(idx);
		
		switch (item?.Type) {
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

		if (slot is not null) {
			ChangeEquipment(slot, item, equip);
			if (equip) {
				Inventory.RemoveItem(idx);
				Equipment.EquipItem(item);
			}
			else {
				if (rIdx == -1)
					Inventory.AddItem(item);
				else {
					Inventory.PutItem(item, rIdx);
				}
				Equipment.RemoveItem(idx);
			}
		}
	}
	
	private void SetupSignals() {
		var ec = GetTree().Root.GetNode<VFlowContainer>("World/InventoryUI/%EquipmentContainer");
		var ic = GetTree().Root.GetNode<GridContainer>("World/InventoryUI/%InventoryContainer/InventoryGrid");

		foreach (var child in ec.GetChildren()) {
			if (child is not EquipmentSlot slot) continue;
			slot.Connect(SlotBase.SignalName.EquipmentChanged,
				new Callable(this, nameof(OnEquipmentChanged)));
		}
		foreach (var child in ic.GetChildren()) {
			if (child is not InventorySlot slot) continue;
			slot.Connect(SlotBase.SignalName.EquipmentChanged,
				new Callable(this, nameof(OnEquipmentChanged)));
		}
	}
	
	public State GetState() => _state;
	public Inventory GetInventory() => Inventory;
	public Equipment GetEquipment() => Equipment;
	private float DistanceTo(Node3D node) => node != null ? GlobalPosition.DistanceTo(node.GlobalPosition) : Mathf.Inf;
	public bool IsValidTarget(Entity enemy) => !enemy.IsDead() && DistanceTo(enemy) < MaxLockonDistance && enemy != _lockedTarget;
	public void ToggleHealthBar(bool visible) => GetNode<HealthBar3D>("Skeleton3D/Head/HealthBar3D").Visible = visible;
	public void BanGravity() => _gravity = 0;
	
	
	private void BuildInventory() {
		Inventory.AddItem(CreateItem("Iron Dagger"));
		Inventory.AddItem(CreateItem("Iron Double Axe"));
		Inventory.AddItem(CreateItem("Archer's Hood"));
		Inventory.AddItem(CreateItem("Archer's Bandolier"));
		Inventory.AddItem(CreateItem("Wooden Shield"));
		Inventory.AddItem(CreateItem("Magical Sword"));
		Inventory.AddItem(CreateItem("Magical Sword"));
		
		Item pot = CreateItem("Minor Health Potion");
		pot.Count = 97;
		Inventory.AddItem(pot);
	}
}
