using System.Collections.Generic;

namespace EuroBooks.Application.Common.Models
{
    public class DataTableResponse
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<object> data { get; set; }
    }
}
