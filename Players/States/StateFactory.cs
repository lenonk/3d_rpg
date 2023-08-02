using Godot;
using System;
using System.Collections.Generic;

public partial class StateFactory : Node {
	public enum State {
		IdleState,
		MoveState,
		JumpState,
		AttackState
	}
	
	private static Dictionary<string, State> StateNames = new Dictionary<string, State> {
		{"Idle", State.IdleState},
		{"Walk", State.MoveState},
		{"Jump", State.JumpState},
		{"Attack", State.AttackState},
	};

	public static bool HasState(string name) => StateNames.ContainsKey(name);
	public static State GetSate(string name) => StateNames[name];
}
