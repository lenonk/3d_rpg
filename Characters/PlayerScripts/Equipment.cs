using System;
using System.Collections.Generic;
using System.Linq;
using static Items;

public class Equipment {
	public const int MaxSize = 5;

	private List<Item> _equipment = new(Enumerable.Repeat<Item>(null, MaxSize));

	public void EquipItem(Item item) => _equipment[(short)item.Type] = item;
	public void RemoveItem(int idx) => _equipment[idx] = null;
	public Item At(int idx) => _equipment[idx];
	public List<Item> GetItems() => _equipment;
}
