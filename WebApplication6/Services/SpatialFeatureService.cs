using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication6.DTOs; // DTO'lar� da yeniden adland�raca��z
using WebApplication6.Interfaces;
using WebApplication6.Models;

namespace WebApplication6.Services
{
    public class SpatialFeatureService : ISpatialFeatureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SpatialFeatureService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FeatureDto> AddAsync(CreateFeatureDto createDto)
        {
            var newFeature = _mapper.Map<SpatialFeature>(createDto);

            await _unitOfWork.Features.AddAsync(newFeature);
            await _unitOfWork.CompleteAsync(); // De�i�iklikleri kaydet!

            return _mapper.Map<FeatureDto>(newFeature);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var feature = await _unitOfWork.Features.GetByIdAsync(id);
            if (feature == null) return false;

            await _unitOfWork.Features.DeleteAsync(feature);
            await _unitOfWork.CompleteAsync(); // De�i�iklikleri kaydet!

            return true;
        }

        // Di�er metotlar da benzer �ekilde g�ncellenir...
        public async Task<List<FeatureDto>> GetAllAsync()
        {
            var features = await _unitOfWork.Features.GetAllAsync();
            return _mapper.Map<List<FeatureDto>>(features);
        }

        public async Task<FeatureDto> GetByIdAsync(int id)
        {
            var feature = await _unitOfWork.Features.GetByIdAsync(id);
            return _mapper.Map<FeatureDto>(feature);
        }

        public async Task<FeatureDto> UpdateAsync(int id, CreateFeatureDto updateDto)
        {
            var existingFeature = await _unitOfWork.Features.GetByIdAsync(id);
            if (existingFeature == null) return null;

            _mapper.Map(updateDto, existingFeature);
            // EF Core tracking sayesinde UpdateAsync �a��rmaya gerek yok, ama yine de �a��rabiliriz.
            // await _unitOfWork.Features.UpdateAsync(existingFeature); 
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<FeatureDto>(existingFeature);
        }
    }
}