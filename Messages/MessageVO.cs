using Invoice.API.Data.ValueObjects;
using Invoice.API.MessageBus;
using System;
using System.Text.Json.Serialization;

namespace Invoice.API.Messages
{
    public class MessageVO : BaseMessage
    {
        public InvoiceVO invoice { get; set; }
    }
}
