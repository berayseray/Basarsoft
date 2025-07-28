using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication6.DTOs;
using WebApplication6.Interfaces; // ISpatialFeatureService için
using WebApplication6.Models;// ApiResponse için

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        // Controller artýk IUnitOfWork'ü deðil, iþ katmaný olan ISpatialFeatureService'i tanýr.
        private readonly ISpatialFeatureService _featureService;

        public FeaturesController(ISpatialFeatureService featureService)
        {
            _featureService = featureService;
        }

        /// <summary>
        /// Sistemdeki tüm mekansal nesneleri listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var features = await _featureService.GetAllAsync();
            var response = new ApiResponse<List<FeatureDto>> { Data = features, Message = "Tüm nesneler baþarýyla getirildi." };
            return Ok(response);
        }

        /// <summary>
        /// Verilen ID'ye sahip tek bir mekansal nesneyi getirir.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var featureDto = await _featureService.GetByIdAsync(id);

            if (featureDto == null)
            {
                return NotFound(new ApiResponse<FeatureDto> { Message = "Belirtilen ID'ye sahip nesne bulunamadý.", IsSuccess = false });
            }

            var response = new ApiResponse<FeatureDto> { Data = featureDto, Message = "Nesne baþarýyla bulundu." };
            return Ok(response);
        }

        /// <summary>
        /// Yeni bir mekansal nesne (Point, Polygon vb.) ekler.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateFeatureDto createDto)
        {
            var addedFeatureDto = await _featureService.AddAsync(createDto);

            // Eðer servis null dönerse, bu, eklenen poligonun mevcut poligonlar tarafýndan
            // tamamen yutulduðu ve geriye bir þey kalmadýðý anlamýna gelir.
            // Bu bir hata deðildir. Ýstemciye iþlemin yapýldýðýný ama
            // yeni bir kaynak oluþturulmadýðýný bildirmek için 204 No Content dönebiliriz.
            if (addedFeatureDto == null)
            {
                return NoContent(); // HTTP 204
            }

            var response = new ApiResponse<FeatureDto> { Data = addedFeatureDto, Message = "Nesne baþarýyla eklendi." };

            return CreatedAtAction(nameof(GetById), new { id = addedFeatureDto.Id }, response);
        }

        /// <summary>
        /// Mevcut bir mekansal nesneyi günceller.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateFeatureDto updateDto)
        {
            var resultFeatureDto = await _featureService.UpdateAsync(id, updateDto);

            if (resultFeatureDto == null)
            {
                return NotFound(new ApiResponse<FeatureDto> { Message = "Güncellenecek nesne bulunamadý.", IsSuccess = false });
            }

            var response = new ApiResponse<FeatureDto> { Data = resultFeatureDto, Message = "Nesne baþarýyla güncellendi." };
            return Ok(response);
        }

        /// <summary>
        /// Verilen ID'ye sahip mekansal nesneyi siler.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _featureService.DeleteAsync(id);

            if (!isSuccess)
            {
                return NotFound(new ApiResponse<object> { Message = "Silinecek nesne bulunamadý.", IsSuccess = false });
            }

            return Ok(new ApiResponse<object> { Message = "Nesne baþarýyla silindi." });
        }
    }
}