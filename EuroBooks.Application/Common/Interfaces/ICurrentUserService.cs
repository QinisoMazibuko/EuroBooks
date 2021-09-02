using System;
using System.Collections.Generic;
using System.Text;

namespace EuroBooks.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        long UserId { get; }
    }
}
