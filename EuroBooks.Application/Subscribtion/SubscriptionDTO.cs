using AutoMapper;
using EuroBooks.Application.Book;
using EuroBooks.Application.Common.Mappings;
using EuroBooks.Application.User;

namespace EuroBooks.Application.Subscribtion
{
    public class SubscriptionDTO : IMapFrom<Domain.Enities.Subscription>
    {
        public long Id { get; set; }
        public long UserID { get; set; }
        public long BookID { get; set; }

        public UserDTO User { get; set; }
        public BookDTO Book { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Enities.Subscription, SubscriptionDTO>();
        }
    }
}
