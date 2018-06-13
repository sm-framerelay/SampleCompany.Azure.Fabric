using System;
using System.Runtime.Serialization;

namespace SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase
{
    [DataContract]
    public class OrderDetailsDto
    {
        [DataMember]
        public Guid OrderId { get; set; }

        [DataMember]
        public Guid ItemId { get; set; }
    }
}
