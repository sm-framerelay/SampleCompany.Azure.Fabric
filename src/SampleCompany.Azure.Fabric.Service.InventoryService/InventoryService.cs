using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SampleCompany.Azure.Fabric.Service.InventoryService.Interfaces;
using SampleCompany.Azure.Fabric.Shared;

namespace SampleCompany.Azure.Fabric.Service.InventoryService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class InventoryService : StatefulService, IInventoryService
    {
        public InventoryService(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                ServiceEventSource.Current.ServiceMessage(this, "Inside RunAsync for Inventory Service");
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(this, "RunAsync Failed, {0}", e);
                throw;
            }
        }


        public Task<bool> IsItemInInventoryAsync(Guid itemId, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<int> AddStockAsync(Guid itemId, int quantity)
        {
            return Task.FromResult(1);
        }

        public Task<int> RemoveStockAsync(Guid itemId, int quantity, OrderActorMessageId messageId)
        {
            return Task.FromResult(1);
        }
    }
}
