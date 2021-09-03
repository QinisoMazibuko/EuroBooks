using EuroBooks.Domain.Common;
using System.Collections.Generic;

namespace EuroBooks.Domain.Enities
{
    public class Book : AuditableEntity
    {
        public Book()
        {
            Subscribers = new List<Subscription>();
        }

        public string Name { get; set; }
        public string Text { get; set; }
        public double PurchasePrice { get; set; }

        public ICollection<Subscription> Subscribers { get; private set; }
    }
}
