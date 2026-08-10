using System.Collections.Generic;

namespace StellaGame;

public class Inventory
{
    public List<ItemStack> Items { get; set; } = new();

    public class ItemStack
    {
        public string ItemId { get; set; }

        public int Quantity { get; set; }

        public ItemStack(string itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }

    public void AddItem(string itemId, int quantity)
    {
        AddItem(new ItemStack(itemId, quantity));
    }

    public void AddItem(ItemStack newItemStack)
    {
        var existingItem = Items.Find(item => item.ItemId == newItemStack.ItemId);
        if (existingItem != null)
        {
            existingItem.Quantity += newItemStack.Quantity;
            return;
        }
        var nullIndex = Items.FindIndex(item => item == null);
        if (nullIndex != -1)
        {
            Items[nullIndex] = newItemStack;
            return;
        }
        Items.Add(newItemStack);
    }

    public void RemoveItem(string itemId, int quantity)
    {
        RemoveItem(new ItemStack(itemId, quantity));
    }

    public void RemoveItem(ItemStack subtractItemStack)
    {
        var existingItem = Items.Find(item => item.ItemId == subtractItemStack.ItemId);
        if (existingItem != null)
        {
            existingItem.Quantity -= subtractItemStack.Quantity;
            if (existingItem.Quantity <= 0)
            {
                existingItem = null;
            }
        }
    }

    public int CheckForItem(string itemId, int quantity)
    {
        return CheckForItem(new ItemStack(itemId, quantity));
    }

    public int CheckForItem(ItemStack subtractItemStack)
    {
        var existingItem = Items.FindIndex(item => item.ItemId == subtractItemStack.ItemId);
        return existingItem != -1 && Items[existingItem].Quantity >= subtractItemStack.Quantity ? existingItem : -1;
    }
}
