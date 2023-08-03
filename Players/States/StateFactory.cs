using Godot;
using System;
using System.Collections.Generic;

public partial class StateFactory : Node {
	public enum StateType {
		IdleState,
		WalkState,
		JumpState,
		AttackState
	}
	
	private static Dictionary<string, StateType> _stateNames = new() {
		{"Idle", StateType.IdleState},
		{"Walk", StateType.WalkState},
		{"Jump", StateType.JumpState},
		{"Attack", StateType.AttackState},
	};
	
	public static State GetInstance(string stateName) {
		if (!HasState(stateName))
			return null;

		Type t = Type.GetType(_stateNames[stateName].ToString());
	    return (State)Activator.CreateInstance(t);
	}
	
	public static bool HasState(string name) => _stateNames.ContainsKey(name);
	public static StateType GetSate(string name) => _stateNames[name];
}
