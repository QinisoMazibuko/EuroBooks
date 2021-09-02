using System.Collections.Generic;

namespace EuroBooks.Application.Common.Models
{
    /// <summary>
    /// <c>DataTableParams</c> class to handle DataTable Events
    /// </summary>
    /// <remarks>
    /// Data being POST from DataTableJS(new versions)
    /// </remarks>
    public class DataTableParams
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<DtOrder> order { get; set; }
        public DtSearch search { get; set; }
        public List<DtColumn> columns { get; set; }
    }

    public class DtOrder
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public class DtColumn
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public DtSearch search { get; set; }
    }


    public class DtSearch
    {
        public string value { get; set; }
        public bool regex { get; set; }
    }
}
