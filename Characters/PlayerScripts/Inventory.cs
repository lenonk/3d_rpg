using Godot;
using System.Collections.Generic;
using System.Collections.Concurrent;
using static Items;
using System;

public partial class Inventory : Node {
	public const int MaxSize = 78;
	
	private ConcurrentDictionary<string, Item> _inventory = new();

	public void RemoveItem(Item i) {
		if (_inventory.Remove(i.Name, out Item temp))
			if (--temp.Count >= 0) AddItem(temp);
	}

	public void AddItem(Item i) {
		_inventory.AddOrUpdate(i.Name, i, (key, oldItem) =>
		{
			oldItem.Count++;
			return oldItem;
		});
	}

	public int CountItem(Item i) => _inventory.TryGetValue(i.Name, out var item) ? item.Count : 0;
	public int CountItem(string i) => _inventory.TryGetValue(i, out var item) ? item.Count : 0;

	public bool GetItem(Item i, out Item item) {
		if (_inventory.TryGetValue(i.Name, out var value)) {
			item = value;
			return true;	
		}

		item = null;
		return false;
	}
	
	public bool GetItem(string i, out Item item) {
		if (_inventory.TryGetValue(i, out var value)) {
			item = value;
			return true;	
		}

		item = null;
		return false;
	}

	public List<Item> GetItems() {
		List<Item> list = new();

		foreach (KeyValuePair<string,Item> item in _inventory) {
			list.Add(item.Value);	
		}

		return list;
	}
}
