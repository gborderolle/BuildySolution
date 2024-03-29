namespace BuildyBackend.Core.Helpers;

public static class Messages
{
    public static class Generic
    {
        public const string NotValid = "Datos de entrada inválidos.";
        public const string InternalError = "Ocurrió un error en el servidor.";
        public const string NameAlreadyExists = "El nombre {0} ya existe en el sistema.";
    }

    public static class Estate
    {
        public const string NotFoundGeneric = "El sistema no tiene Propiedad asignado.";
        public const string NotValid = "Datos de entrada inválidos para Propiedad.";
        public const string NotFound = "Propiedad no encontrado Id: {0}.";
        public const string Created = "Propiedad creado con éxito.";
        public const string Updated = "Propiedad actualizado correctamente Id: {0}.";
        public const string Deleted = "Propiedad eliminado con éxito.";
        public const string ActionLog = "Propiedad Id: {0}, Nombre: {1}";
    }

    public static class Job
    {
        public const string NotValid = "Obra de entrada inválidos para Obra.";
        public const string NotFound = "Obra no encontrada Id: {0}.";
        public const string Created = "Obra creada con éxito Id: {0}.";
        public const string Updated = "Obra actualizada correctamente Id: {0}.";
        public const string Deleted = "Obra eliminada con éxito.";
        public const string ActionLog = "Obra Id: {0}, Nombre: {1}";
    }
    public static class Rent
    {
        public const string NotValid = "Datos de entrada inválidos para Renta.";
        public const string NotFound = "Renta no encontrada Id: {0}.";
        public const string Created = "Renta creada con éxito Id: {0}.";
        public const string Updated = "Renta actualizada correctamente Id: {0}.";
        public const string Deleted = "Renta eliminada con éxito.";
        public const string ActionLog = "Renta Id: {0}, Nombre: {1}";
    }
    public static class Report
    {
        public const string NotValid = "Datos de entrada inválidos para Reporte.";
        public const string NotFound = "Reporte no encontrado Id: {0}.";
        public const string Created = "Reporte creado con éxito Id: {0}.";
        public const string Updated = "Reporte actualizado correctamente Id: {0}.";
        public const string Deleted = "Reporte eliminado con éxito.";
        public const string ActionLog = "Reporte Id: {0}, Nombre: {1}";
    }
    public static class Tenant
    {
        public const string NotValid = "Datos de entrada inválidos para Inquilino.";
        public const string NotFound = "Inquilino no encontrado Id: {0}.";
        public const string Created = "Inquilino creado con éxito Id: {0}.";
        public const string Updated = "Inquilino actualizado correctamente Id: {0}.";
        public const string Deleted = "Inquilino eliminado con éxito.";
        public const string ActionLog = "Inquilino Id: {0}, Nombre: {1}";
    }

    public static class Worker
    {
        public const string NotValid = "Datos de entrada inválidos para Trabajador.";
        public const string NotFound = "Trabajador no encontrado Id: {0}.";
        public const string Created = "Trabajador creado con éxito Id: {0}.";
        public const string Updated = "Trabajador actualizado correctamente Id: {0}.";
        public const string Deleted = "Trabajador eliminado con éxito.";
        public const string ActionLog = "Trabajador Id: {0}, Nombre: {1}";
    }

    public static class City
    {
        public const string NotValid = "Datos de entrada inválidos para Ciudad.";
        public const string NotFound = "Ciudad no encontrada Id: {0}.";
        public const string Created = "Ciudad creada con éxito Id: {0}.";
        public const string Updated = "Ciudad actualizada correctamente Id: {0}.";
        public const string Deleted = "Ciudad eliminada con éxito.";
        public const string ActionLog = "Ciudad Id: {0}, Nombre: {1}";
    }

    public static class Province
    {
        public const string NotValid = "Datos de entrada inválidos para Departamento.";
        public const string NotFound = "Departamento no encontrada Id: {0}.";
        public const string Created = "Departamento creada con éxito Id: {0}.";
        public const string Updated = "Departamento actualizada correctamente Id: {0}.";
        public const string Deleted = "Departamento eliminada con éxito.";
        public const string ActionLog = "Departamento Id: {0}, Nombre: {1}";
    }

    public static class Country
    {
        public const string NotValid = "Datos de entrada inválidos para País.";
        public const string NotFound = "País no encontrada Id: {0}.";
        public const string Created = "País creada con éxito Id: {0}.";
        public const string Updated = "País actualizada correctamente Id: {0}.";
        public const string Deleted = "País eliminada con éxito.";
        public const string ActionLog = "País Id: {0}, Nombre: {1}";
    }

}
