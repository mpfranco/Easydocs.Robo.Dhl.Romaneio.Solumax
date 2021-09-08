using System;

namespace Easydocs.Robo.Dhl.Romaneio.Solumax.Infrastructure.Shared.Messages
{
    public abstract class Message
    {
        public string MessageType { get; protected set; }
        public Guid AggregateId { get; protected set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}
