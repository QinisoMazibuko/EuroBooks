using EuroBooks.Domain.Enums;

namespace EuroBooks.Application.Common.Models
{
    /// <summary>
    /// <c>PagingInfo</c> class for pagination
    /// </summary>
    public class PagingInfo
    {
        public int skip { get; set; } = 0;
        public int take { get; set; } = 10;
        public int resultCount { get; set; }

        //default filter
        public ActiveState activeState { get; set; } = ActiveState.ActiveOnly;

        //searching
        public string searchString { get; set; }

        //sorting
        public string sortBy { get; set; }
        public bool isSortAsc { get; set; } = true;

        public PagingInfo() { }

        /// <summary>
        /// Translate <see cref="DataTableParams"/> object into <c>PagingInfo</c> object
        /// </summary>
        public PagingInfo(DataTableParams dataTableParams)
        {
            if (dataTableParams != null)
            {
                searchString = dataTableParams.search?.value;
                take = dataTableParams.length;
                skip = dataTableParams.start;

                if (dataTableParams.order != null)
                {
                    sortBy = dataTableParams.columns[dataTableParams.order[0].column].data;
                    isSortAsc = dataTableParams.order[0].dir.ToLower() == "asc";
                }

            }
        }

        public PagingInfo(int skip, int take, string searchString)
        {
            this.skip = skip;
            this.take = take;
            this.searchString = searchString;
        }

        public PagingInfo(int skip, int take, string searchString, string sortBy, bool isSortAsc)
        {
            this.skip = skip;
            this.take = take;
            this.searchString = searchString;
            this.sortBy = sortBy;
            this.isSortAsc = isSortAsc;
        }

        public PagingInfo(int skip, int take, string searchString, string sortBy, bool isSortAsc, ActiveState activeState)
        {
            this.skip = skip;
            this.take = take;
            this.searchString = searchString;
            this.activeState = activeState;
            this.sortBy = sortBy;
            this.isSortAsc = isSortAsc;
        }

    }
}
