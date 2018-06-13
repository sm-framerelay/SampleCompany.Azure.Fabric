using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SampleCompany.Azure.Fabric.Contracts.Data.Request.Inventory;
using SampleCompany.Azure.Fabric.Contracts.Data.Response.Inventory;
using SampleCompany.Azure.Fabric.Service.InventoryService.Interfaces;
using SampleCompany.Azure.Fabric.Shared;

namespace SampleCompany.Azure.Fabric.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private const string InventoryServiceNameKey = "InventoryService";

        [HttpPost]
        public async Task<ActionResult<AddInventoryResponse>> AddNew(
            AddInventoryRequest request,
            CancellationToken cancellationToken)
        {
            if (null == request?.Inventory)
                throw new ArgumentNullException(nameof(request.Inventory));

            AddInventoryResponse response;

            var builder = new SharedUriBuilder(InventoryServiceNameKey);
            var inventoryServiceClient = ServiceProxy.Create<IInventoryService>(builder.ToUri());

            try
            {
                var status = await inventoryServiceClient.CreateInventoryItemAsync(request.Inventory);
                response = new AddInventoryResponse {Status = status};
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message("There are one or more errors while creating inventory {0}: {1}",
                    request.Inventory.Id, ex);
                throw;
            }

            return response;
        }
    }
}
