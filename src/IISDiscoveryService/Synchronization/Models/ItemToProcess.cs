namespace IISDiscoveryService.Synchronization.Models
{
    public class ItemToProcess<T>
    {
        public bool Processed { get; set; }
        public T Item { get; set; }
    }
}