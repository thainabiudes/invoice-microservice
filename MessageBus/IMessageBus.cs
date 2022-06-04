using System.Threading.Tasks;

namespace Invoice.API.MessageBus
{
    public interface IMessageBus
    {
        Task PublicMessage(BaseMessage message, string queueName);
    }
}
