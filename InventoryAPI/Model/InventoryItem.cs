﻿namespace InventoryAPI.Model
{
    public class InventoryItem
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
