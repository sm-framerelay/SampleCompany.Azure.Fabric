using System.Collections.Generic;
using System.Runtime.Serialization;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase;

namespace SampleCompany.Azure.Fabric.Contracts.Data.Request.Purchase
{
    [DataContract]
    public class PurchaseOrderRequest
    {
        [DataMember]
        public IList<OrderDto> Orders { get; set; }
    }
}
