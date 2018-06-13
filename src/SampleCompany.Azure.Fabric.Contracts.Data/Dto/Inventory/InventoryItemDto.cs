using System;
using System.Runtime.Serialization;

namespace SampleCompany.Azure.Fabric.Contracts.Data.Dto.Inventory
{
    [DataContract]
    public class InventoryItemDto
    {
        /// <summary>
        /// Inventory unique identity
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Quantity in stock
        /// </summary>
        [DataMember]
        public int AvailableStock { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// Brief description of product
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Available stock at which we should reorder
        /// </summary>
        [DataMember]
        public int RestockThreshold { get; set; }

        /// <summary>
        /// Maximum number of units due to physicial/logistical constraints in warehouses
        /// </summary>
        [DataMember]
        public int MaxStockThreshold { get; set; }

        /// <summary>
        /// True if item is on reorder
        /// </summary>
        [DataMember]
        public bool OnReorder { get; set; }
    }
}
