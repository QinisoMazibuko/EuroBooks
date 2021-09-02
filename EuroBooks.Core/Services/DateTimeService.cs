using EuroBooks.Application.Common.Interfaces;
using System;

namespace EuroBooks.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
