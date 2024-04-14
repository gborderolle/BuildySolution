namespace BuildyBackend.Core.DTO
{
    public class OwnerDTO
    {
        #region Internal

        public int Id { get; set; }
        public required string Name { get; set; }
        public string Color { get; set; }
        public DateTime Creation { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public string NominatimCityCode { get; set; }

        #endregion

        #region External

        #endregion
    }
}
