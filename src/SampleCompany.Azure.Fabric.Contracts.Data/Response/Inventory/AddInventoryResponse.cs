using System.Runtime.Serialization;

namespace SampleCompany.Azure.Fabric.Contracts.Data.Response.Inventory
{
    [DataContract]
    public class AddInventoryResponse
    {
        [DataMember]
        public bool Status { get; set; }
    }
}
