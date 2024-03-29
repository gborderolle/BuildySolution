using AutoMapper;
using BuildyBackend.Core.DTO;
using BuildyBackend.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BuildyBackend.Core.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<IdentityUser, UserDTO>();

            CreateMap<Estate, EstateDTO>()
              .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
              .ForMember(dest => dest.ListReports, opt => opt.MapFrom(src => src.ListReports))
              .ForMember(dest => dest.ListJobs, opt => opt.MapFrom(src => src.ListJobs))
              .ForMember(dest => dest.ListRents, opt => opt.MapFrom(src => src.ListRents))
              .ReverseMap();
            CreateMap<EstateCreateDTO, Estate>()
              .ForMember(dest => dest.City, opt => opt.Ignore()) // Ignorar este campo
              .ForMember(dest => dest.Owner, opt => opt.Ignore()) // Ignorar este campo
              .ReverseMap();

            CreateMap<WorkerCreateDTO, Worker>()
              .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.JobId.HasValue ? src.JobId.Value : (int?)null))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.JobId.HasValue ? src.JobId.Value : (int?)null))
              .ReverseMap();
            CreateMap<Worker, WorkerDTO>().ReverseMap();

            CreateMap<TenantCreateDTO, Tenant>().ReverseMap();
            CreateMap<Tenant, TenantDTO>().ReverseMap();

            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<CountryCreateDTO, Country>().ReverseMap();

            CreateMap<Province, ProvinceDTO>()
              .ForMember(dest => dest.CountryId,
                opt => opt.MapFrom(src => src.Country.Id))
              .ReverseMap();
            CreateMap<ProvinceDSCreateDTO, Province>()
              .ForMember(dest => dest.Country, opt => opt.Ignore()) // Ignorar este campo
              .ReverseMap();

            CreateMap<City, CityDTO>()
              .ForMember(dest => dest.ProvinceId,
                opt => opt.MapFrom(src => src.Province.Id))
              .ReverseMap();
            CreateMap<CityCreateDTO, City>()
              .ForMember(dest => dest.Province, opt => opt.Ignore()) // Ignorar este campo
              .ReverseMap();

            CreateMap<Owner, OwnerDTO>()
        .ReverseMap();
            CreateMap<OwnerCreateDTO, Owner>()
              .ReverseMap();

            CreateMap<Report, ReportDTO>()
              .ForMember(dest => dest.ListPhotosURL, opt => opt.MapFrom(src => src.ListPhotos.Select(photo => photo.URL).ToList()))
            .ReverseMap();
            CreateMap<ReportCreateDTO, Report>()
              .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignorar Id ya que es generado por la base de datos
              .ForMember(dest => dest.ListPhotos, opt => opt.Ignore()) // Ignorar porque lo agrego a mano en el Controller
              .ReverseMap();

            CreateMap<Rent, RentDTO>()
                .ForMember(destinationMember: dest => dest.ListFilesURL, opt => opt.MapFrom(src => src.ListFiles.Select(file => file.URL).ToList()))
                .ReverseMap();

            CreateMap<RentCreateDTO, Rent>()
              .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignorar Id ya que es generado por la base de datos
              .ForMember(dest => dest.ListTenants, opt => opt.Ignore()) // Ignorar porque lo agrego a mano en el Controller
              .ForMember(dest => dest.ListFiles, opt => opt.Ignore()) // Ignorar porque lo agrego a mano en el Controller
              .ReverseMap();

            CreateMap<Job, JobDTO>()
              .ForMember(dest => dest.ListPhotosURL, opt => opt.MapFrom(src => src.ListPhotos.Select(photo => photo.URL).ToList()))
            .ReverseMap();
            CreateMap<JobCreateDTO, Job>()
              .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignorar Id ya que es generado por la base de datos
              .ForMember(dest => dest.ListWorkers, opt => opt.Ignore())
              .ForMember(dest => dest.ListPhotos, opt => opt.Ignore()) // Ignorar porque lo agrego a mano en el Controller
              .ReverseMap();

        }

    }
}