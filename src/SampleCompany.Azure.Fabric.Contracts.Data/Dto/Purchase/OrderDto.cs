using System.Runtime.Serialization;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Inventory;

namespace SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase
{
    [DataContract]
    public class OrderDto
    {
        [DataMember]
        public InventoryItemDto Item { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public int Remaining { get; set; }
    }
}
