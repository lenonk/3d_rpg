using Godot;
using System;

public partial class Dagger : WeaponBase {
	public override void _Ready() {
		base._Ready();
		Damage = 2;
	}
}
