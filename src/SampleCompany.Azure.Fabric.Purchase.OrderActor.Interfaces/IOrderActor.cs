using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase;

[assembly: FabricTransportActorRemotingProvider(RemotingListener = RemotingListener.V2Listener, RemotingClient = RemotingClient.V2Client)]
namespace SampleCompany.Azure.Fabric.Purchase.OrderActor.Interfaces
{
    /// <summary>
    /// Order Actor which places submitted orders
    /// </summary>
    public interface IOrderActor : IActor
    {
        /// <summary>
        /// Process orders passed in parameters
        /// </summary>
        /// <param name="orders">Orders to process</param>
        /// <param name="cancellationToken">Cancellation token to drop operation if needed</param>
        /// <returns>Task to execute</returns>
        Task SubmitOrderAsync(List<OrderDto> orders, CancellationToken cancellationToken);
    }
}
