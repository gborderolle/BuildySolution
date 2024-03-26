using AutoMapper;
using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.DTO;

namespace BuildyBackend.Infrastructure.Services
{
    public class CountryResolverService : IValueResolver<ProvinceDSCreateDTO, ProvinceDS, CountryDS>
    {
        private readonly ICountryResolver _countryResolver;

        public CountryResolverService(ICountryResolver countryResolver)
        {
            _countryResolver = countryResolver;
        }

        public CountryDS Resolve(ProvinceDSCreateDTO source, ProvinceDS destination, CountryDS destMember, ResolutionContext context)
        {
            return _countryResolver.ResolveCountry(source.CountryDSId);
        }
    }
}
