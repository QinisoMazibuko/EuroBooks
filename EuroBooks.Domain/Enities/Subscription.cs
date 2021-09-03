using EuroBooks.Domain.Common;

namespace EuroBooks.Domain.Enities
{
    public class Subscription : AuditableEntity
    {
        public Subscription()
        {
           
        }

        public long UserID { get; set; }
        public long BookID { get; set; }

    }
}
