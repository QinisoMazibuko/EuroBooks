using System;

namespace EuroBooks.Domain.Common
{
    public class AuditableEntity
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }

        public long CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public long? LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
