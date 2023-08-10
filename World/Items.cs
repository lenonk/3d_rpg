#nullable enable
using Godot;
using System.Collections.Generic;

public partial class Items : Node {

	public enum ItemType {
		None,
		Head,
		Body,
		Weapon,
		Shield,
		Waist,
		Misc
	};
	
	public record ItemPrototype(
		string Name, 
		string Description, 
		int Value, 
		ItemType Type, 
		string Icon,
		string Scene
	);
	
	public class Item {
		// [Obsolete("Do not use the default constructor.", true)]
		public Item() {
			_item = null;
			Count = -1;
			Index = -1;
		}
		
		private Item(ItemPrototype proto) {
			_item = proto;
			Count = 1;
			Index = 0;
		}

		public static bool operator ==(Item i1, Item i2) => i1.Name == i2.Name;
		public static bool operator !=(Item i1, Item i2) => i1.Name != i2.Name;
		public static Item CreateItem(string name) => new(GameItems[name]);
		public static bool IsItem(string name) => GameItems.ContainsKey(name);
		
		private readonly ItemPrototype? _item;
		public int Index;
		public int Count;
		
		public string Name        { get => _item?.Name ?? ""; }
		public string Description { get => _item?.Description ?? ""; }
		public string Icon        { get => _item?.Icon ?? ""; }
		public int Value          { get => _item?.Value ?? 0; }
		public ItemType Type          { get => _item?.Type ?? ItemType.None; }
	}

	public static Item CreateItem(string name) => Item.CreateItem(name);
	public static bool IsItem(string name) => Item.IsItem(name);
	
	private static readonly Dictionary<string, ItemPrototype> GameItems = new() {
		{"Iron Dagger", new(
			"Iron Dagger", 
			"A common iron dagger.", 
			100, 
			ItemType.Weapon,
			"res://Assets/Icons/iron_dagger.png",
			"res://Characters/Weapons/Dagger/Dagger.tscn")
		},
		{"Iron Double Axe", new(
			"Iron Double Axe", 
			"A common iron double axe.", 
			150,
			ItemType.Weapon,
			"res://Assets/Icons/iron_double_axe.png",
			"res://Characters/Weapons/DoubleAxe/DoubleAxe.tscn")
			
		},
		{"Magical Sword", new(
			"Magical Sword", 
			"A magical sword.", 
			10000,
			ItemType.Weapon,
			"res://Assets/Icons/magical_sword.png",
			"res://Characters/Weapons/Dagger/Dagger.tscn")
		},
	};
}
