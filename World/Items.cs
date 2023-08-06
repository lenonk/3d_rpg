using Godot;
using System.Collections.Generic;

public partial class Items : Node {

	public record ItemPrototype(string Name, string Description, int Value, string Icon);
	
	public struct Item {
		public Item(ItemPrototype proto) {
			_item = proto;
			Count = Index = 0;
		}
		
		private ItemPrototype _item;
		public int Index;
		public int Count;
		
		public string Name        { get => _item.Name; }
		public string Description { get => _item.Description; }
		public string Icon        { get => _item.Icon; }
		public int Value          { get => _item.Value; }
	}

	public static Item CreateItem(string name) => new(_gameItems[name]);
	
	private static Dictionary<string, ItemPrototype> _gameItems = new() {
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
