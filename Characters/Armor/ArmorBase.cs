using Godot;
using System;

public partial class ArmorBase : EquipmentBase
{
	protected Entity Wearer;

	public override void _Ready() {
		Node3D parent = GetParent() as Node3D;

		while (parent != null && parent is not Entity)
			parent = parent.GetParent() as Node3D;

		if (parent != null)
			Wearer = parent as Entity;
	}	
}
