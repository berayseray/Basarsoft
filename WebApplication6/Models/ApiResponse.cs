
using AutoMapper.Features;

namespace WebApplication6.Models
{
    // Bu sınıf, API'den dönen tüm cevapları standart bir yapıda tutar.
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;

        public static implicit operator ApiResponse<T>(ApiResponse<List<SpatialFeature>> v)
        {
            throw new NotImplementedException();
        }
    }
}
