using Godot;
using System.Collections.Generic;
using static Items;
using System;

public partial class Inventory : Node {
	public const int MaxSize = 78;
	
	private Dictionary<string, Item> _inventory = new();

	public void RemoveItem(Item i) {
		if (_inventory.Remove(i.Name, out Item temp))
			if (--temp.Count >= 0) AddItem(temp);
	}
	
	public void AddItem(Item i) => _inventory.TryAdd(i.Name, i);
	
	public int CountItem(Item i) => _inventory[i.Name].Count;
	public int CountItem(string i) => _inventory[i].Count;

	public Item GetItem(Item i) => _inventory[i.Name];
	public Item GetItem(string i) => _inventory[i];

	public List<Item> GetItems() {
		List<Item> list = new();

		foreach (KeyValuePair<string,Item> item in _inventory) {
			list.Add(item.Value);	
		}

		return list;
	}
}
