namespace InventoryAPI.Model
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public DateTime ShipmentDate { get; set; }

    }
}
