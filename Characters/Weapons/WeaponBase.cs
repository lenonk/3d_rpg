using Godot;
using System;

public partial class WeaponBase : EquipmentBase
{
	[Export] public int Damage = 0;

	protected Entity Wielder;

	public override void _Ready() {
		Node3D parent = GetParent() as Node3D;

		while (parent != null && parent is not Entity)
			parent = parent.GetParent() as Node3D;

		if (parent != null)
			Wielder = parent as Entity;
	}
	
	private void OnHit(Node3D body) {
		if (body is not Entity entity) return;

		if (entity.IsInGroup("Entities") && entity != Wielder) {
			entity.TakeDamage(Damage);
		}
	}
}
