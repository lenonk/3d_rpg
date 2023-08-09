#nullable enable
using Godot;
using System;
using System.Collections.Generic;

public partial class Items : Node {

	public record ItemPrototype(string Name, string Description, int Value, string Icon);
	
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
	}

	public static Item CreateItem(string name) => Item.CreateItem(name);
	public static bool IsItem(string name) => Item.IsItem(name);
	
	private static readonly Dictionary<string, ItemPrototype> GameItems = new() {
		{"Wooden Sword", new(
			"Wooden Sword", 
			"A wooden sword.", 
			100, 
			"res://Assets/Icons/wooden_sword.png")
		},
		{"White Sword", new(
			"White Sword", 
			"A white sword.", 
			1000,
			"res://Assets/Icons/white_sword.png")
			
		},
		{"Magical Sword", new(
			"Magical Sword", 
			"A magical sword.", 
			10000,
			"res://Assets/Icons/magical_sword.png")
		},
	};
}
