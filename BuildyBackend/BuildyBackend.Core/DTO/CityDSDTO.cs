namespace BuildyBackend.Core.DTO
{
    public class CityDTO
    {
        #region Internal

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Creation { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public string NominatimCityCode { get; set; }

        #endregion

        #region External

        public int ProvinceId { get; set; }

        #endregion
    }
}
