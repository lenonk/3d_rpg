using Godot;
using System;

public partial class Entity : CharacterBody3D {
	[Export] public int MaxHealth = 10;
	[Export] public int Health = 10;

	[Signal] public delegate void EntityDiedEventHandler();
	
	public float Size;

	public override void _Ready() {
		CollisionShape3D cShape = GetNode<CollisionShape3D>("CollisionShape3D");
		Size = ((CapsuleShape3D)cShape.Shape).Radius;
	}

	public virtual void TakeDamage(int value) {
		if (Health > 0 && (Health -= value) <= 0)
			Die();
	}

	public virtual bool IsDead() {
		return Health <= 0;
	}
	
	public virtual void Die() {
		EmitSignal(SignalName.EntityDied);
	}
}
