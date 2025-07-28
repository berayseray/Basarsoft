using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication6.Models
{
    // Bu sınıf artık sadece bir noktayı değil, herhangi bir mekansal nesneyi temsil ediyor.
    public class SpatialFeature
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Bu 'Geometry' propertysi, bir Point, Polygon, LineString veya başka bir
        // NTS geometrisini tutabilir. İşte bu polimorfizmdir.
        [Column(TypeName = "geometry")]
        public Geometry Location { get; set; }
    }
}