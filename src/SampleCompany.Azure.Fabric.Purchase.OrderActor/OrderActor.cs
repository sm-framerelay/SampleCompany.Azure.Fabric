using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase;
using SampleCompany.Azure.Fabric.Purchase.OrderActor.Interfaces;
using SampleCompany.Azure.Fabric.Service.InventoryService.Interfaces;
using SampleCompany.Azure.Fabric.Shared;

namespace SampleCompany.Azure.Fabric.Purchase.OrderActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class OrderActor : Actor, IOrderActor,  IRemindable
    {
        private const string OrdersKey = "Orders";
        private const string OrderStatusKey = "OrderStatus";
        private const string FulfillOrderReminderKey = "FulfillOrderReminder";
        private const string RequestIdPropertyKey = "RequestId";
        private const string InventoryServiceNameKey = "InventoryService";

        // Reminder settings
        private const int ReminderDueTimeoutInSeconds = 12;
        private const int ReminderPeriodTimeoutInSeconds = 12;

        private IServiceProxyFactory _serviceProxyFactory;
        private SharedUriBuilder _builder;
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// Initializes a new instance of OrderActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public OrderActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            _tokenSource = new CancellationTokenSource();
            _builder = new SharedUriBuilder(ActorService.Context.CodePackageActivationContext, InventoryServiceNameKey);
            _serviceProxyFactory = new ServiceProxyFactory();

            var orderStatusResult = await GetOrderStatusAsync();

            // Init order if it's new
            if (orderStatusResult == OrderStatusTypeDto.Unknown)
            {
                await StateManager.SetStateAsync(OrdersKey, new List<OrderDto>());
                await StateManager.SetStateAsync<long>(RequestIdPropertyKey, 0);
                await SetOrderStatusAsync(OrderStatusTypeDto.New);
            }
        }

        protected override Task OnDeactivateAsync()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            return Task.FromResult(true);
        }

        public async Task SubmitOrderAsync(List<OrderDto> orders, CancellationToken cancellationToken)
        {
            if (null == orders?.Count)
            {
                ActorEventSource.Current.Message("The submitted order is empty, nothing to process. Terminated.");
                return;
            }

            try
            {
                await StateManager.SetStateAsync(OrdersKey, new List<OrderDto>(orders), cancellationToken);
                await StateManager.SetStateAsync(OrderStatusKey, OrderStatusTypeDto.Submitted, cancellationToken);

                await RegisterReminderAsync(
                    FulfillOrderReminderKey,
                    null,
                    TimeSpan.FromSeconds(ReminderDueTimeoutInSeconds),
                    TimeSpan.FromSeconds(ReminderPeriodTimeoutInSeconds));
            }
            catch (Exception e)
            {
                ActorEventSource.Current.Message(e.ToString());
            }

            ActorEventSource.Current.Message("Order submitted with {0} items", orders.Count);
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            // Check reminder name
            if (FulfillOrderReminderKey != reminderName)
            {
                throw new InvalidOperationException("Unknown reminder key: " + reminderName);
            }

            await ExecuteOrderAsync();

            var orderStatus = await GetOrderStatusAsync();

            if (orderStatus == OrderStatusTypeDto.Shipped || orderStatus == OrderStatusTypeDto.Canceled)
            {
                // Remove reminder to garbage collect the Actor
                var orderReminder = GetReminder(FulfillOrderReminderKey);
                await UnregisterReminderAsync(orderReminder);
            }
        }

        public async Task<string> GetOrderStatusAsStringAsync()
        {
            return (await GetOrderStatusAsync()).ToString();
        }

        private async Task ExecuteOrderAsync()
        {
            await SetOrderStatusAsync(OrderStatusTypeDto.InProcess);

            var orderedItems = await StateManager.GetStateAsync<List<OrderDto>>(OrdersKey);

            ActorEventSource.Current.ActorMessage(this, "Executing customer order. ID: {0}. Items: {1}", Id.GetGuidId(), orderedItems.Count);

            foreach (var item in orderedItems)
            {
                ActorEventSource.Current.Message("Order contains:{0}", item);
            }

            // Throught all ordered items 
            foreach (var item in orderedItems.Where(x => x.Remaining > 0))
            {
                var inventoryService = _serviceProxyFactory.CreateServiceProxy<IInventoryService>(_builder.ToUri());

                // Check the item is listed in inventory
                if (await inventoryService.IsItemInInventoryAsync(item.Item.Id, _tokenSource.Token) == false)
                {
                    await SetOrderStatusAsync(OrderStatusTypeDto.Canceled);
                    return;
                }

                var numberItemsRemoved =
                    await
                        inventoryService.RemoveStockAsync(
                            item.Item.Id,
                            item.Quantity,
                            new OrderActorMessageId(
                                new ActorId(Id.GetGuidId()),
                                await StateManager.GetStateAsync<long>(RequestIdPropertyKey)));

                item.Remaining -= numberItemsRemoved;
            }

            var items = await StateManager.GetStateAsync<IList<OrderDto>>(OrdersKey);
            bool backordered = false;

            // Set proper status
            foreach (var item in items)
            {
                if (item.Remaining > 0)
                {
                    backordered = true;
                    break;
                }
            }

            if (backordered)
            {
                await SetOrderStatusAsync(OrderStatusTypeDto.Backordered);
            }
            else
            {
                await SetOrderStatusAsync(OrderStatusTypeDto.Shipped);
            }

            ActorEventSource.Current.ActorMessage(
                this,
                "{0}; Executed: {1}. Backordered: {2}",
                await GetOrderStatusAsStringAsync(),
                items.Count(x => x.Remaining == 0),
                items.Count(x => x.Remaining > 0));

            long messageRequestId = await StateManager.GetStateAsync<long>(RequestIdPropertyKey);
            await StateManager.SetStateAsync(RequestIdPropertyKey, ++messageRequestId);
        }

        private async Task<OrderStatusTypeDto> GetOrderStatusAsync()
        {
            var orderStatusResult = await StateManager.TryGetStateAsync<OrderStatusTypeDto>(OrderStatusKey);
            if (orderStatusResult.HasValue)
            {
                return orderStatusResult.Value;
            }

            return OrderStatusTypeDto.Unknown;
        }

        private async Task SetOrderStatusAsync(OrderStatusTypeDto orderStatus)
        {
            await StateManager.SetStateAsync(OrderStatusKey, orderStatus);
        }
    }
}
