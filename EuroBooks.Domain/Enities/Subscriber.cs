using EuroBooks.Domain.Common;
using System.Collections.Generic;

namespace EuroBooks.Domain.Enities
{
    public class Subscriber : AuditableEntity
    {
        public Subscriber()
        {
            Subscriptions = new List<Subscription>();
        }

        public long UserID { get; set; }

        public ICollection<Subscription> Subscriptions { get; private set; }

    }
}
