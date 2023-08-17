using Godot;
using System;

public partial class WeaponBase : EquipmentBase
{
	[Export] public int Damage = 0;

	protected Entity Wielder;

	public override void _Ready() {
		Wielder = FindOwner();
	}
	
	private void OnHit(Node3D body) {
		if (body is not Entity entity) return;

		if (entity.IsInGroup("Entities") && entity != Wielder) {
			entity.TakeDamage(Damage);
		}
	}
	
	private Entity FindOwner() {
		var parent = GetParent();
		
		while (parent != null && parent is not Entity)
			parent = parent.GetParent();

		return parent as Entity;
	}
}
