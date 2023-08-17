using System;
using System.Collections.Generic;
using System.Linq;
using static Items;

public partial class Equipment {
	public const int MaxSize = 5;

	private enum SlotTypes : short {
		Head = 0,
		Body = 1,
		Weapon = 2,
		Shield = 3,
		Waist = 4
	};
	
	private List<Item> _equipment = new(Enumerable.Repeat<Item>(null, MaxSize));

	public void EquipItem(Item item) {
		switch (item.Type) {
			case ItemType.Head:
				_equipment[(short)SlotTypes.Head] = item;
				break;
			case ItemType.Body:
				_equipment[(short)SlotTypes.Body] = item;
				break;
			case ItemType.Weapon:
				_equipment[(short)SlotTypes.Weapon] = item;
				break;
			case ItemType.Shield:
				_equipment[(short)SlotTypes.Shield] = item;
				break;
			case ItemType.Waist:
				_equipment[(short)SlotTypes.Waist] = item;
				break;
		}
	}

	public void RemoveItem(int idx) => _equipment[idx] = null;
	
	public Item At(int idx) => _equipment[idx];
	public List<Item> GetItems() => _equipment;
}
