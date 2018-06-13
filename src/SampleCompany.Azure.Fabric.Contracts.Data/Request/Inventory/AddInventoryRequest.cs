using System.Runtime.Serialization;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Inventory;

namespace SampleCompany.Azure.Fabric.Contracts.Data.Request.Inventory
{
    [DataContract]
    public class AddInventoryRequest
    {
        [DataMember]
        public InventoryItemDto Inventory { get; set; }
    }
}
