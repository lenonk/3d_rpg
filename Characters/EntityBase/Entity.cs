using Godot;
using System;

public partial class Entity : CharacterBody3D {
	public float Size;

	public override void _Ready() {
		CollisionShape3D cShape = GetNode<CollisionShape3D>("CollisionShape3D");
		Size = ((CapsuleShape3D)cShape.Shape).Radius;
	}

	public virtual void TakeDamage(int value) {
	}
}
