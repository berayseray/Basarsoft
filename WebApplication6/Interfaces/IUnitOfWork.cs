using System;
using System.Threading.Tasks;

namespace WebApplication6.Interfaces
{
    // IDisposable, DbContext'in doğru şekilde dispose edilmesini sağlar.
    public interface IUnitOfWork : IDisposable
    {
        // Her repository için bir property tanımlıyoruz.
        ISpatialFeatureRepository Features { get; }

        // Tüm değişiklikleri veritabanına kaydetmek için tek bir metot.
        Task<int> CompleteAsync();
    }
}