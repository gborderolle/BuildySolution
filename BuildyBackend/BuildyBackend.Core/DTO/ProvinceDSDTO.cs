namespace BuildyBackend.Core.DTO
{
    public class ProvinceDTO
    {
        #region Internal

        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime Creation { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public string NominatimProvinceCode { get; set; }

        #endregion

        #region External

        public int CountryId { get; set; }

        #endregion

    }
}
