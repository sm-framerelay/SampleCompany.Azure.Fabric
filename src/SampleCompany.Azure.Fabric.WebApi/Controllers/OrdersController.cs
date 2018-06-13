using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase;
using SampleCompany.Azure.Fabric.Contracts.Data.Response.Purchase;
using SampleCompany.Azure.Fabric.Purchase.OrderActor.Interfaces;
using SampleCompany.Azure.Fabric.Shared;

namespace SampleCompany.Azure.Fabric.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private const string OrderActorServiceKey = "OrderActorService";

        [HttpGet]
        public async Task<ActionResult<PurchaseOrderResponse>> GetOrders()
        {
            return new PurchaseOrderResponse
            {
                OrderId = Guid.NewGuid()
            };
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseOrderResponse>> PlaceOrder(List<OrderDto> orders, CancellationToken cancellationToken)
        {
            var orderId = Guid.NewGuid();
            var builder = new SharedUriBuilder(OrderActorServiceKey);

            // Calls Actor creation using unique order Id identifier
            var orderProxy = ActorProxy.Create<IOrderActor>(new ActorId(orderId), builder.ToUri());
            try
            {
                await orderProxy.SubmitOrderAsync(orders, cancellationToken);
                ServiceEventSource.Current.Message("Order has been submitted successfully. Actor with OrderId: {0} has been created", orderId);
            }
            catch (InvalidOperationException ex)
            {
                ServiceEventSource.Current.Message("Actor service: Actor rejected {0}: {1}", orders, ex);
                throw;
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message("Actor service: Exception {0}: {1}", orders, ex);
                throw;
            }

            return new PurchaseOrderResponse
            {
                OrderId = orderId
            };
        }
    }
}
