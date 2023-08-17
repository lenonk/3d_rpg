using System;
using System.Collections.Generic;
using System.Linq;
using static Items;

public partial class Inventory {
	public const int MaxSize = 60;

	private List<Item> _inventory = new(Enumerable.Repeat<Item>(null, MaxSize));

	public void AddItem(Item i) {
		Item tmp;
		if ((tmp = _inventory.Find(item => item == i)) != null && i.IsStackable()) {
			if (tmp.Count + i.Count <= i.StackSize) {
				tmp.Count += i.Count;
				return;
			}
		}

		int idx = _inventory.FindIndex(item => item == null);
		if (idx == -1) {} // TODO: Handle inventory overflow
		_inventory[idx] = i;
	}

	public void PutItem(Item i, int idx) {
		while (_inventory[idx] != null && idx < MaxSize)
			idx++;

		if (idx >= MaxSize)
			return;

		_inventory[idx] = i;
	}
	
	public void MoveItem(int fromIdx, int toIdx, int count = 0) {
		fromIdx = Math.Clamp(fromIdx, 0, MaxSize);
		toIdx = Math.Clamp(toIdx, 0, MaxSize);

		if (fromIdx == toIdx) return;

		if (At(fromIdx).IsStackable() && At(fromIdx) == At(toIdx))
			count = At(fromIdx).Count;
		
		if (count <= 0) {
			(_inventory[fromIdx], _inventory[toIdx]) = (_inventory[toIdx], _inventory[fromIdx]);
			return;
		}

		if (_inventory[toIdx] == null) {
			_inventory[toIdx] = CreateItem(_inventory[fromIdx].Name);
			_inventory[toIdx].Count = 0;
		}

		count = Math.Clamp(count, 1, At(fromIdx).Count);
		_inventory[fromIdx].Count -= count;
		_inventory[toIdx].Count += count;

		if (_inventory[fromIdx].Count == 0)
			_inventory[fromIdx] = null;
	}
	
	public bool GetItem(Item i, out Item retval) {
		if (_inventory.Contains(i)) {
			retval = _inventory[_inventory.IndexOf(i)];
			return true;
		}

		retval = null;
		return false;
	}

	public void RemoveItem(Item i) => _inventory[_inventory.IndexOf(i)] = null;
	public void RemoveItem(int idx) => _inventory[idx] = null;
	public int CountItem(Item i) => _inventory.FindAll(item => item.Name == i.Name).Count;
	public int CountItem(string i) => _inventory.FindAll(item => item.Name == i).Count;
	public Item At(int idx) => _inventory[idx];
	public List<Item> GetItems() => _inventory;
	
}
