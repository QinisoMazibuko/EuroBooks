using EuroBooks.Domain.Common;

namespace EuroBooks.Domain.Enities
{
    public class ApplicationVariables : AuditableEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Environment { get; set; }
    }
}
