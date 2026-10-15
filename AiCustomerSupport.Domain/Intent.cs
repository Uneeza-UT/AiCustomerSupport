using AiCustomerSupport.Domain.Common;

namespace AiCustomerSupport.Domain
{
    public class Intent : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
