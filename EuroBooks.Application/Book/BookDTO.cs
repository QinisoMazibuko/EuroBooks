using AutoMapper;
using EuroBooks.Application.Common.Mappings;
using EuroBooks.Application.User;
using System.Collections.Generic;

namespace EuroBooks.Application.Book
{
    public class BookDTO : IMapFrom<Domain.Enities.Book>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public double PurchasePrice { get; set; }

        public List<UserDTO> Subscribers { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Enities.Book, BookDTO>();
        }
    }
}
