using AutoMapper;
using EuroBooks.Application.Common.Interfaces;
using EuroBooks.Application.Common.Models;
using EuroBooks.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace EuroBooks.Application.Book.Queries
{
    public class GetBookListQuery : IRequest<List<BookDTO>>
    {
        public PagingInfo paging { get; set; }
        public class GetBookListQueryHandler : IRequestHandler<GetBookListQuery, List<BookDTO>>
        {
            public readonly IApplicationDbContext context;
            public readonly IMapper mapper;

            public GetBookListQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<List<BookDTO>> Handle(GetBookListQuery request, CancellationToken cancellationToken)
            {
                #region Query
                var books = from row in context.Books
                            where row.IsActive
                            select new BookDTO
                            {
                                Id = row.Id,
                                Name = row.Name,
                                Text = row.Text,
                                PurchasePrice = row.PurchasePrice
                            };
                #endregion

                #region Filtering

                string search = request.paging.searchString;
                if (!string.IsNullOrEmpty(search)) books = books.Where(
                    r => r.Name.Contains(search) ||
                    r.Text.Contains(search));

                #endregion Filtering

                #region Sorting

                string orderByCol = request.paging.sortBy;
                bool orderAscending = request.paging.isSortAsc;

                if (!string.IsNullOrWhiteSpace(orderByCol))
                {
                    switch (orderByCol)
                    {
                        case ("name"):
                            {
                                if (orderAscending)
                                    books = books.OrderBy(r => r.Name);
                                else
                                    books = books.OrderByDescending(r => r.Name);
                            }
                            break;
                    }
                }

                #endregion Sorting

                #region Meterialization 
                request.paging.resultCount = books.Count();

                var qCount = books.DeferredCount().FutureValue();
                var qFuture = books.Skip(request.paging.skip).Take(request.paging.take).Future();

                var result = await qFuture.ToListAsync();
                request.paging.resultCount = qCount.Value;
                #endregion

                return result;
            }
        }
    }
}