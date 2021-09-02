using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<EuroBooks.Domain.Enities.ApplicationVariables> ApplicationVariables { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
