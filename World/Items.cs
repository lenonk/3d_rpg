using Godot;
using System;
using System.Collections.Generic;

public partial class Items : Node {

	public enum ItemType {
		Head,
		Body,
		Weapon,
		Shield,
		Waist,
		Misc,
		None
	};
	
	public record ItemPrototype(
		string Name, 
		string Description, 
		int Value, 
		int StackSize,
		ItemType Type, 
		string Icon,
		string Scene
	);
	
	public partial class Item : Node {
		[Obsolete("Do not use the default constructor.", true)]
		public Item() {
			_item = null;
			Count = -1;
			Index = -1;
			IsWearing = false;
		}
		
		private Item(ItemPrototype proto) {
			_item = proto;
			Count = 1;
			Index = 0;
			IsWearing = false;
		}

		public static Item CreateItem(string name) => new(GameItems[name]);
		public static bool IsItem(string name) => GameItems.ContainsKey(name);
		
		private readonly ItemPrototype? _item;
		public int Index;
		public int Count;
		public bool IsWearing;
		
		public string Name        { get => _item?.Name ?? ""; }
		public string Description { get => _item?.Description ?? ""; }
		public string Icon        { get => _item?.Icon ?? ""; }
		public string Scene       { get => _item?.Scene ?? ""; }
		public int Value          { get => _item?.Value ?? 0; }
		public int StackSize      { get => _item?.StackSize ?? 0; }
		public ItemType Type      { get => _item?.Type ?? ItemType.None; }

		public bool IsWearable() => Type != ItemType.Misc && Type != ItemType.None;
		public bool IsStackable() => StackSize > 1;
		
		public static bool operator ==(Item i1, Item i2) => i1?.Name == i2?.Name;
		public static bool operator !=(Item i1, Item i2) => i1?.Name != i2?.Name;
	}

	public static Item CreateItem(string name) => Item.CreateItem(name);
	public static bool IsItem(string name) => Item.IsItem(name);
	
	private static readonly Dictionary<string, ItemPrototype> GameItems = new() {
		{"Iron Dagger", new(
			"Iron Dagger", 
			"A common iron dagger.", 
			100, 
			1,
			ItemType.Weapon,
			"res://Assets/Icons/iron_dagger.png",
			"res://Characters/Weapons/Dagger/Dagger.tscn")
		},
		{"Iron Double Axe", new(
			"Iron Double Axe", 
			"A common iron double axe.", 
			150,
			1,
			ItemType.Weapon,
			"res://Assets/Icons/iron_double_axe.png",
			"res://Characters/Weapons/DoubleAxe/DoubleAxe.tscn")
			
		},
		{"Magical Sword", new(
			"Magical Sword", 
			"A magical sword.", 
			10000,
			1,
			ItemType.Weapon,
			"res://Assets/Icons/magical_sword.png",
			"res://Characters/Weapons/Dagger/Dagger.tscn")
		},
		{"Archer's Hood", new(
			"Archer's Hood", 
			"An identity concealing hood made for archers.", 
			1000,
			1,
			ItemType.Head,
			"res://Assets/Icons/archer_hood.png",
			"res://Characters/Armor/ArcherHood/ArcherHood.tscn")
		},
		{"Archer's Bandolier", new(
			"Archer's Bandolier", 
			"An bandolier made for archers. It has many pockets and a quiver for your arrows.", 
			1000,
			1,
			ItemType.Body,
			"res://Assets/Icons/archer_body.png",
			"res://Characters/Armor/ArcherBody/ArcherBody.tscn")
		},
		{"Wooden Shield", new(
			"Wooden Shield", 
			"A shield. Made of wood.", 
			1000,
			1,
			ItemType.Shield,
			"res://Assets/Icons/shield_common.png",
			"res://Characters/Armor/WoodenShield/WoodenShield.tscn")
		},
		{"Minor Health Potion", new(
			"Minor Health Potion", 
			"A health potion that does nothing.", 
			1,
			99,
			ItemType.Misc,
			"res://Assets/Icons/minor_health_potion.png",
			"")
		},
	};
}
