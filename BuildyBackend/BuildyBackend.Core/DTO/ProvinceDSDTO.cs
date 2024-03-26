namespace BuildyBackend.Core.DTO
{
    public class ProvinceDSDTO
    {
        #region Internal

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Creation { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public string NominatimProvinceCode { get; set; }

        #endregion

        #region External

        public int CountryDSId { get; set; }

        #endregion

    }
}
