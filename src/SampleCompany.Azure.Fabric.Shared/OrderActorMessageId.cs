using System;
using System.Runtime.Serialization;
using Microsoft.ServiceFabric.Actors;

namespace SampleCompany.Azure.Fabric.Shared
{
    [DataContract]
    public class OrderActorMessageId : IFormattable, IComparable, IComparable<OrderActorMessageId>,
        IEquatable<OrderActorMessageId>
    {
        [DataMember]
        public ActorId SendingActorId { get; private set; }

        [DataMember]
        public long MessageId { get; private set; }

        public OrderActorMessageId(ActorId sendingActorId, long messageId)
        {
            SendingActorId = sendingActorId;
            MessageId = messageId;
        }

        int IComparable.CompareTo(object obj)
        {
            return CompareTo((OrderActorMessageId) obj);
        }

        public int CompareTo(OrderActorMessageId other)
        {
            if (string.Compare(SendingActorId.ToString(), other.SendingActorId.ToString(), StringComparison.Ordinal) >
                1)
            {
                return 1;
            }

            if (string.Compare(SendingActorId.ToString(), other.SendingActorId.ToString(), StringComparison.Ordinal) <
                1)
            {
                return -1;
            }

            if (MessageId > other.MessageId)
            {
                return 1;
            }

            if (MessageId < other.MessageId)
            {
                return -1;
            }

            return 0;
        }

        public bool Equals(OrderActorMessageId other)
        {
            return other != null && (SendingActorId.Equals(other.SendingActorId) && MessageId == other.MessageId);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{SendingActorId}|{MessageId}";
        }

        public static OrderActorMessageId GetRandom()
        {
            ActorId id = new ActorId(Guid.NewGuid());
            Random r = new Random();
            return new OrderActorMessageId(id, r.Next());
        }

        public static bool operator ==(OrderActorMessageId item1, OrderActorMessageId item2)
        {
            return item1 != null && item1.Equals(item2);
        }

        public static bool operator !=(OrderActorMessageId item1, OrderActorMessageId item2)
        {
            return item1 != null && !item1.Equals(item2);
        }

        public static bool operator >(OrderActorMessageId item1, OrderActorMessageId item2)
        {
            int result = item1.CompareTo(item2);
            return (result == 0 | result == -1);
        }

        public static bool operator <(OrderActorMessageId item1, OrderActorMessageId item2)
        {
            int result = item1.CompareTo(item2);
            return result == 0 | result == 1;
        }

        public override bool Equals(object obj)
        {
            return (CompareTo(obj as OrderActorMessageId) == 0);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
