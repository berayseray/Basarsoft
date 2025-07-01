using Microsoft.AspNetCore.Mvc;

namespace Webapplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase
    {
        private static readonly List<Point> _pointList = new List<Point>();

        [HttpGet]
        public List<Point> GetAllPoint()
        {
            return _pointList;
        }

        [HttpPost]
        public Point AddPoint(Point p)
        {
            _pointList.Add(p);
            return p;
        }
        // --- YENÝ ÝSTENEN METOTLAR ---

        // GET: api/point/5
        [HttpGet("{id}")]
        public ApiResponse<Point> GetById(int id)
        {
            var point = _pointList.FirstOrDefault(p => p.Id == id);

            if (point == null)
            {
                return new ApiResponse<Point> { Message = "Belirtilen ID'ye sahip nokta bulunamadý." };
            }

            return new ApiResponse<Point> { Data = point, Message = "Nokta bulundu." };
        }

        // PUT: api/point/5
        [HttpPut("{id}")]
        public ApiResponse<Point> Update(int id, Point updatedPoint)
        {
            var existingPoint = _pointList.FirstOrDefault(p => p.Id == id);

            if (existingPoint == null)
            {
                return new ApiResponse<Point> { Message = "Güncellenecek nokta bulunamadý." };
            }

            existingPoint.Name = updatedPoint.Name;
            // Diðer özellikler de burada güncellenir...

            return new ApiResponse<Point> { Data = existingPoint, Message = "Nokta baþarýyla güncellendi." };
        }

        // DELETE: api/point/5
        [HttpDelete("{id}")]
        public ApiResponse<object> Delete(int id) // Dönen bir veri olmadýðý için "object" kullandýk.
        {
            var pointToRemove = _pointList.FirstOrDefault(p => p.Id == id);

            if (pointToRemove == null)
            {
                return new ApiResponse<object> { Message = "Silinecek nokta bulunamadý." };
            }

            _pointList.Remove(pointToRemove);

            return new ApiResponse<object> { Message = "Nokta baþarýyla silindi." };
        }
    }

    // Point sýnýfý
    public class Point
    {
        public int Id { get; set; }
        public double PointX { get; set; }
        public double PointY { get; set; }
        public string Name { get; set; }
    }
}