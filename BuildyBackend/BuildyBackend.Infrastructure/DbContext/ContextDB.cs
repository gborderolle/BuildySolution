using BuildyBackend.Core;
using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.IdentityEntities;
using BuildyBackend.Core.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuildyBackend.Infrastructure.DbContext
{
    public class ContextDB : IdentityDbContext
    {
        public ContextDB(DbContextOptions<ContextDB> options) : base(options)
        {
        }

        #region DB Tables

        public DbSet<BuildyUser> BuildyUser { get; set; }
        public DbSet<BuildyRole> BuildyRole { get; set; }
        public DbSet<Estate> Estate { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<File1> File { get; set; }
        public DbSet<Rent> Rent { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<Worker> Worker { get; set; }
        public DbSet<CityDS> CityDS { get; set; }
        public DbSet<ProvinceDS> ProvinceDS { get; set; }
        public DbSet<CountryDS> CountryDS { get; set; }
        public DbSet<OwnerDS> OwnerDS { get; set; }
        public DbSet<Log> Log { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1..N
            modelBuilder.Entity<Report>()
                .HasMany(r => r.ListPhotos)
                .WithOne(p => p.Report)
                .HasForeignKey(p => p.ReportId);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            SeedUsers(modelBuilder);
            SeedEntities(modelBuilder);
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/20660148#notes
            // Generar GUID: https://guidgenerator.com/online-guid-generator.aspx
            // ---------------- Usuarios ---------------------------------------------
            var userAdminId = "c2ee6493-5a73-46f3-a3f2-46d1d11d7176";
            var rolAdminId = "bef4cbd4-1f2b-472f-a1e2-e1a901f6808c";

            var userUserId = "e0765c93-676c-4199-b7ee-d7877c471821";
            var rolUserId = "bef4cbd4-1f2b-472f-a3f2-e1a901f6811c";

            var rolAdmin = new BuildyRole
            {
                Id = rolAdminId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Creation = DateTime.Now,
                Update = DateTime.Now
            };

            var rolUser = new BuildyRole
            {
                Id = rolUserId,
                Name = "User",
                NormalizedName = "USER",
                Creation = DateTime.Now,
                Update = DateTime.Now
            };

            var passwordHasher = new PasswordHasher<BuildyUser>();

            var username1 = "useradmin";
            var email1 = "admin@buildy.lat";
            var userAdmin = new BuildyUser()
            {
                Id = userAdminId,
                UserName = username1,
                NormalizedUserName = username1.ToUpper(),
                Email = email1,
                NormalizedEmail = email1.ToUpper(),
                PasswordHash = passwordHasher.HashPassword(null, "useradmin1234"),
                Name = "Usuario administrador"
            };

            var username2 = "usernormal";
            var email2 = "normal@buildy.lat";
            var userUser = new BuildyUser()
            {
                Id = userUserId,
                UserName = username2,
                NormalizedUserName = username2.ToUpper(),
                Email = email2,
                NormalizedEmail = email2.ToUpper(),
                PasswordHash = passwordHasher.HashPassword(null, "usernormal1234"),
                Name = "Usuario normal"
            };

            var username3 = "mirtadls";
            var email3 = "mirta@buildy.lat";
            var id3 = "58fbedfc-e682-479b-ba46-19ef4c137d2a";
            var userMirta = new BuildyUser()
            {
                Id = id3,
                UserName = username3,
                NormalizedUserName = username3.ToUpper(),
                Email = email3,
                NormalizedEmail = email3.ToUpper(),
                PasswordHash = passwordHasher.HashPassword(null, "mirtadls1234"),
                Name = "Mirta de los Santos"
            };

            var username4 = "irgla";
            var email4 = "gladys@buildy.lat";
            var id4 = "11c767dc-e8ce-448e-8fdb-ee590a44a3ff";
            var userGladys = new BuildyUser()
            {
                Id = id4,
                UserName = username4,
                NormalizedUserName = username4.ToUpper(),
                Email = email4,
                NormalizedEmail = email4.ToUpper(),
                PasswordHash = passwordHasher.HashPassword(null, "irgla1234"),
                Name = "Gladys de los Santos"
            };

            modelBuilder.Entity<BuildyUser>()
                .HasData(userAdmin, userUser, userMirta, userGladys);

            modelBuilder.Entity<BuildyRole>()
                .HasData(rolAdmin, rolUser);

            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasData(new IdentityUserClaim<string>()
                {
                    Id = 1,
                    ClaimType = "role",
                    UserId = userAdminId,
                    ClaimValue = "admin"
                });

            modelBuilder.Entity<IdentityUserClaim<string>>()
    .HasData(new IdentityUserClaim<string>()
    {
        Id = 2,
        ClaimType = "role",
        UserId = userUserId,
        ClaimValue = "user"
    });

            // Asignar roles a usuarios
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = rolAdminId,
                    UserId = userAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = rolUserId,
                    UserId = userUserId
                },
                new IdentityUserRole<string>
                {
                    RoleId = rolUserId,
                    UserId = userMirta.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = rolUserId,
                    UserId = userGladys.Id
                }
            );
        }

        private void SeedEntities(ModelBuilder modelBuilder)
        {
            var country1 = new CountryDS() { Id = 1, Name = "Uruguay", NominatimCountryCode = "UY" };
            modelBuilder.Entity<CountryDS>().HasData(new List<CountryDS>
            {
                country1
            });

            var province1 = new ProvinceDS() { Id = 1, Name = "Cerro Largo", CountryDSId = 1, NominatimProvinceCode = "CL" };
            var province2 = new ProvinceDS() { Id = 2, Name = "Montevideo", CountryDSId = 1, NominatimProvinceCode = "MO" };
            modelBuilder.Entity<ProvinceDS>().HasData(new List<ProvinceDS>
            {
                province1,province2
            });

            var city1 = new CityDS { Id = 1, Name = "Melo", ProvinceDSId = 1, NominatimCityCode = "ME" };
            var city2 = new CityDS() { Id = 2, Name = "Montevideo", ProvinceDSId = 2, NominatimCityCode = "MO" };
            modelBuilder.Entity<CityDS>().HasData(new List<CityDS>
            {
                city1,city2
            });

            var owner1 = new OwnerDS() { Id = 1, Name = "Mirta", Color = "violet" };
            var owner2 = new OwnerDS() { Id = 2, Name = "Gladys", Color = "orange" };
            var owner3 = new OwnerDS() { Id = 3, Name = "Cristina", Color = "green" };
            modelBuilder.Entity<OwnerDS>().HasData(new List<OwnerDS>
            {
                owner1,owner2,owner3
            });

            // Estates ****************
            // Calle Colón

            var estateColon1 = new Estate() { Id = 1, Name = "Colón 476", Address = "Colón 476", CityDSId = 1, OwnerDSId = 3, LatLong = "-32.3674814,-54.1757085", GoogleMapsURL = "https://www.google.com/maps/place/C.+Col%C3%B3n+476,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3674814,-54.1757085,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b6fbe707:0x3f99b6b98961385b!8m2!3d-32.3674814!4d-54.1731282!16s%2Fg%2F11fn9p0pws?hl=es-419&entry=ttu" };

            var estateColon2 = new Estate() { Id = 2, Name = "Colón 480", Address = "Colón 480", CityDSId = 1, OwnerDSId = 2, LatLong = "-32.3674762,-54.1755751", GoogleMapsURL = "https://www.google.com/maps/place/C.+Col%C3%B3n+480,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3674762,-54.1755751,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b0c5f12b:0xaf5b5ba2ea4eb1cb!8m2!3d-32.3674762!4d-54.1729948!16s%2Fg%2F11rz98nq5_?hl=es-419&entry=ttu" };

            var estateColon3 = new Estate() { Id = 3, Name = "Colón 491", Address = "Colón 491", CityDSId = 1, OwnerDSId = 2, LatLong = "-32.367241,-54.1754153", GoogleMapsURL = "https://www.google.com/maps/place/C.+Col%C3%B3n+491,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.367241,-54.1754153,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b180e29b:0x95542a9eb2345b16!8m2!3d-32.367241!4d-54.172835!16s%2Fg%2F11fctc9z9z?hl=es-419&entry=ttu" };

            var estateColon4 = new Estate() { Id = 4, Name = "Colón 495", Address = "Colón 495", CityDSId = 1, OwnerDSId = 2, LatLong = "-32.3672195,-54.1755857", GoogleMapsURL = "https://www.google.com/maps/place/C.+Col%C3%B3n+495,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3672195,-54.1755857,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b3f659db:0xf3c82a67cd3ebe76!8m2!3d-32.3672195!4d-54.1730054!16s%2Fg%2F11gmz50cjj?hl=es-419&entry=ttu" };

            var estateColon5 = new Estate() { Id = 5, Name = "Colón 503", Address = "Colón 503", CityDSId = 1, OwnerDSId = 2, LatLong = "-32.3672809,-54.1751337", GoogleMapsURL = "https://www.google.com/maps/place/C.+Col%C3%B3n+503,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3672809,-54.1751337,17z/data=!3m1!4b1!4m5!3m4!1s0x95092980a659bd3f:0x1f64283da64670a9!8m2!3d-32.3672809!4d-54.1725534?hl=es-419&entry=ttu" };

            // Calle Darío Silva

            var estateDario1 = new Estate() { Id = 6, Name = "Darío Silva 774", Address = "Darío Silva 774", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3679807,-54.1752022", GoogleMapsURL = "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+774,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3679807,-54.1752022,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980a2c60edb:0x48ad692931a026ea!8m2!3d-32.3679807!4d-54.1726219!16s%2Fg%2F11gr6dzf3g?hl=es-419&entry=ttu" };

            var estateDario2 = new Estate() { Id = 7, Name = "Darío Silva 774 BIS", Address = "Darío Silva 774 BIS", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3679807,-54.1752022", GoogleMapsURL = "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+774,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3679807,-54.1752022,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980a2c60edb:0x48ad692931a026ea!8m2!3d-32.3679807!4d-54.1726219!16s%2Fg%2F11gr6dzf3g?hl=es-419&entry=ttu" };

            var estateDario3 = new Estate() { Id = 8, Name = "Darío Silva 781", Address = "Darío Silva 781", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3675191,-54.1753876", GoogleMapsURL = "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+781,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3675191,-54.1753876,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b076246d:0x1b122d0ac3c1dbc7!8m2!3d-32.3675191!4d-54.1728073!16s%2Fg%2F11g194j1tj?hl=es-419&entry=ttu" };

            var estateDario4 = new Estate() { Id = 9, Name = "Darío Silva 785", Address = "Darío Silva 785", CityDSId = 1, OwnerDSId = 2, LatLong = "-32.3676402,-54.1754579", GoogleMapsURL = "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+785,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3676402,-54.1754579,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980ba5f73d1:0x186b45d8ed124b2a!8m2!3d-32.3676402!4d-54.1728776!16s%2Fg%2F11sb62lb6c?hl=es-419&entry=ttu" };

            var estateDario5 = new Estate() { Id = 10, Name = "Darío Silva 789", Address = "Darío Silva 789", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3676919,-54.1754685", GoogleMapsURL = "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+789,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3676919,-54.1754685,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980ba535f25:0xbdc6468d3ed51cc1!8m2!3d-32.3676919!4d-54.1728882!16s%2Fg%2F11syz1ryh9?hl=es-419&entry=ttu" };

            var estateDario6 = new Estate() { Id = 11, Name = "Darío Silva 793", Address = "Darío Silva 793", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3675108,-54.1754001", GoogleMapsURL = "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+793,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3675108,-54.1754001,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b079be31:0xa19a7a3d822a0595!8m2!3d-32.3675108!4d-54.1728198!16s%2Fg%2F11h_c4rxz9?hl=es-419&entry=ttu" };

            var estateDario7 = new Estate() { Id = 12, Name = "Darío Silva 801", Address = "Darío Silva 801", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3673741,-54.1752804", GoogleMapsURL = "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+801,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3673741,-54.1752804,17z/data=!3m1!4b1!4m5!3m4!1s0x95092980ad44ba05:0x3da437edd983606!8m2!3d-32.3673741!4d-54.1727001?hl=es-419&entry=ttu" };

            var estateDario8 = new Estate() { Id = 13, Name = "Darío Silva 803", Address = "Darío Silva 803", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3673108,-54.1755231", GoogleMapsURL = "https://www.google.com/maps/place/Calle+Dr.+Juan+Dar%C3%ADo+Silva+803,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3673108,-54.1755231,17z/data=!3m1!4b1!4m5!3m4!1s0x95092980ad44ba05:0x24b5f3fc904ac228!8m2!3d-32.3673108!4d-54.1729428?hl=es-419&entry=ttu" };

            var estateDario9 = new Estate() { Id = 14, Name = "Darío Silva Cochera", Address = "Darío Silva Cochera", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3674814,-54.1757085", GoogleMapsURL = "https://www.google.com/maps/place/C.+Col%C3%B3n+476,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3674814,-54.1757085,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980b6fbe707:0x3f99b6b98961385b!8m2!3d-32.3674814!4d-54.1731282!16s%2Fg%2F11fn9p0pws?hl=es-419&entry=ttu" };

            // Otros

            var estateOtros1 = new Estate() { Id = 15, Name = "Treinta y Tres 299", Address = "Treinta y Tres 299", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3765821,-54.1703877", GoogleMapsURL = "https://www.google.com/maps/place/Treinta+y+Tres+299,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3765821,-54.1703877,17z/data=!3m1!4b1!4m6!3m5!1s0x95092bd6c1beccc3:0xb91b3165ec6107e3!8m2!3d-32.3765821!4d-54.1678074!16s%2Fg%2F11gmz7h_cl?hl=es-419&entry=ttu" };

            var estateOtros2 = new Estate() { Id = 16, Name = "Manuel Oribe 788", Address = "Manuel Oribe 788", CityDSId = 1, OwnerDSId = 1, LatLong = "-32.3764414,-54.1706063", GoogleMapsURL = "https://www.google.com/maps/place/C.+Gral.+Manuel+Oribe+788,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3764414,-54.1706063,17z/data=!3m1!4b1!4m6!3m5!1s0x95092bd69533354f:0xd6a6a68977288372!8m2!3d-32.3764414!4d-54.168026!16s%2Fg%2F11h57sb583?hl=es-419&entry=ttu" };

            var estateOtros3 = new Estate() { Id = 17, Name = "Rincón Artigas 702", Address = "Rincón Artigas 702", CityDSId = 1, OwnerDSId = 2, LatLong = "-32.3690096,-54.176412", GoogleMapsURL = "https://www.google.com/maps/place/Dr+Rincon+Artigas+702,+37000+Melo,+Departamento+de+Cerro+Largo/@-32.3690096,-54.176412,17z/data=!3m1!4b1!4m6!3m5!1s0x95092980ee49a557:0xfbaefe3d055f9ab9!8m2!3d-32.3690096!4d-54.1738317!16s%2Fg%2F11h_c4zxvw?hl=es-419&entry=ttu" };

            // Montevideo

            var estateMontevideo1 = new Estate() { Id = 18, Name = "Maldonado 1106", Address = "Maldonado 1106", CityDSId = 2, OwnerDSId = 1, LatLong = "-34.9097969,-56.1945273", GoogleMapsURL = "https://www.google.com/maps/place/Maldonado+1106,+11100+Montevideo,+Departamento+de+Montevideo/@-34.9097969,-56.1945273,17z/data=!3m1!4b1!4m6!3m5!1s0x959f81c066a6fb4d:0xdb3d1d7d172a0f4c!8m2!3d-34.9097969!4d-56.191947!16s%2Fg%2F11fhvn7njf?hl=es-419&entry=ttu" };

            modelBuilder.Entity<Estate>().HasData(
               estateColon1, estateColon2, estateColon3, estateColon4, estateColon5, estateDario1, estateDario2, estateDario3, estateDario4, estateDario5, estateDario6, estateDario7, estateDario8, estateDario9, estateOtros1, estateOtros2, estateOtros3, estateMontevideo1
               );
        }

    }
}