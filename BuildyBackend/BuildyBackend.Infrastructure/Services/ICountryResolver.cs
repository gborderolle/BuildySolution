using BuildyBackend.Infrastructure.DbContext;
using BuildyBackend.Core.Domain.Entities;

namespace BuildyBackend.Infrastructure.Services
{
    public interface ICountryResolver
    {
        Country ResolveCountry(int CountryId);
    }

    public class CountryResolver : ICountryResolver
    {
        private readonly ContextDB _context;

        public CountryResolver(ContextDB context)
        {
            _context = context;
        }

        public Country ResolveCountry(int CountryId)
        {
            return _context.Country.FirstOrDefault(c => c.Id == CountryId);
        }
    }

}
