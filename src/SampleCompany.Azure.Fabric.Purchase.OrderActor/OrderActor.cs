using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase;
using SampleCompany.Azure.Fabric.Purchase.OrderActor.Interfaces;

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

        // Reminder settings
        private const int ReminderDueTimeoutInSeconds = 12;
        private const int ReminderPeriodTimeoutInSeconds = 12;

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
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.CompletedTask;
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

            // TODO: Fill business logic here

            // Remove reminder to garbage collect the Actor
            var orderReminder = GetReminder(FulfillOrderReminderKey);
            await UnregisterReminderAsync(orderReminder);
        }
    }
}
