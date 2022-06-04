using System;

namespace Invoice.API.MessageBus
{
    public class BaseMessage
    {
        public DateTime MessageCreated { get; set; }
    }
}
