using System;
using System.Runtime.Serialization;

namespace SampleCompany.Azure.Fabric.Contracts.Data.Response.Purchase
{
    [DataContract]
    public class PurchaseOrderResponse
    {
        [DataMember]
        public Guid OrderId { get; set; }
    }
}
