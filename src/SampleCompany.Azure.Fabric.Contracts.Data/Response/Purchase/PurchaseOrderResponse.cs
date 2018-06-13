using System.Collections.Generic;
using System.Runtime.Serialization;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase;

namespace SampleCompany.Azure.Fabric.Contracts.Data.Response.Purchase
{
    [DataContract]
    public class PurchaseOrderResponse
    {
        [DataMember]
        public IList<OrderDetailsDto> OrderDetails { get; set; }
    }
}
