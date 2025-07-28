namespace WebApplication6.DTOs
{
    /// <summary>
    /// Bir mekansal nesneyi (Feature) istemciye döndürmek için kullanılan DTO.
    /// </summary>
    public class FeatureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Nesnenin WKT (Well-Known Text) formatındaki tam gösterimi.
        /// </summary>
        public string LocationWkt { get; set; }

        /// <summary>
        /// Nesnenin geometrik tipi (örn: "Point", "Polygon").
        /// </summary>
        public string GeometryType { get; set; }
    }
}