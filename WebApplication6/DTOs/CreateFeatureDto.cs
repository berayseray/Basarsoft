using System.ComponentModel.DataAnnotations;

namespace WebApplication6.DTOs
{
    /// <summary>
    /// Yeni bir mekansal nesne (Feature) oluşturmak veya güncellemek için kullanılan DTO.
    /// Polimorfik olarak Point, Polygon vb. geometrileri WKT formatında kabul eder.
    /// </summary>
    public class CreateFeatureDto
    {
        [Required(ErrorMessage = "Name alanı zorunludur.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name alanı 3 ile 100 karakter arasında olmalıdır.")]
        public string Name { get; set; }

        /// <summary>
        /// Mekansal nesnenin WKT (Well-Known Text) formatındaki gösterimi.
        /// Örnekler:
        /// "POINT (32.83 39.92)"
        /// "POLYGON ((...))"
        /// "LINESTRING (...)"
        /// </summary>
        [Required(ErrorMessage = "LocationWkt alanı zorunludur.")]
        public string LocationWkt { get; set; }
    }
}