using System.Collections.Generic;
using WebApplication6.DTOs;
using WebApplication6.Models;
namespace WebApplication6.Interfaces;
// DTO ve metot isimleri ayn� kalabilir
public interface ISpatialFeatureService
{
    Task<List<FeatureDto>> GetAllAsync(); // DTO ismini de de�i�tirelim
    Task<FeatureDto> GetByIdAsync(int id);
    Task<FeatureDto> AddAsync(CreateFeatureDto createDto);
    Task<FeatureDto> UpdateAsync(int id, CreateFeatureDto updateDto);
    Task<bool> DeleteAsync(int id);
}