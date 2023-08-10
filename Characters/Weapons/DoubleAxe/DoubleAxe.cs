using Godot;
using System;

public partial class DoubleAxe : WeaponBase {
	public override void _Ready() {
		base._Ready();
		Damage = 4;
	}
}
