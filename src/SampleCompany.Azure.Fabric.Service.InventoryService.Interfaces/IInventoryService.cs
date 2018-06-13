using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Inventory;
using SampleCompany.Azure.Fabric.Shared;

namespace SampleCompany.Azure.Fabric.Service.InventoryService.Interfaces
{
    public interface IInventoryService : IService
    {
        Task<bool> IsItemInInventoryAsync(Guid itemId, CancellationToken cancellationToken);
        Task<int> AddStockAsync(Guid itemId, int quantity);
        Task<int> RemoveStockAsync(Guid itemId, int quantity, OrderActorMessageId messageId);
        Task<bool> CreateInventoryItemAsync(InventoryItemDto item);
    }
}
