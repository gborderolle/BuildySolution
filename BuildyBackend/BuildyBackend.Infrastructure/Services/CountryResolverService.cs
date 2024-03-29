using AutoMapper;
using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.DTO;

namespace BuildyBackend.Infrastructure.Services
{
    public class CountryResolverService : IValueResolver<ProvinceDSCreateDTO, Province, Country>
    {
        private readonly ICountryResolver _countryResolver;

        public CountryResolverService(ICountryResolver countryResolver)
        {
            _countryResolver = countryResolver;
        }

        public Country Resolve(ProvinceDSCreateDTO source, Province destination, Country destMember, ResolutionContext context)
        {
            return _countryResolver.ResolveCountry(source.CountryId);
        }
    }
}
