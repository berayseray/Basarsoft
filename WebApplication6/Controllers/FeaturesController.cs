using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication6.DTOs;
using WebApplication6.Interfaces; // ISpatialFeatureService i�in
using WebApplication6.Models;// ApiResponse i�in

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        // Controller art�k IUnitOfWork'� de�il, i� katman� olan ISpatialFeatureService'i tan�r.
        private readonly ISpatialFeatureService _featureService;

        public FeaturesController(ISpatialFeatureService featureService)
        {
            _featureService = featureService;
        }

        /// <summary>
        /// Sistemdeki t�m mekansal nesneleri listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var features = await _featureService.GetAllAsync();
            var response = new ApiResponse<List<FeatureDto>> { Data = features, Message = "T�m nesneler ba�ar�yla getirildi." };
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
                return NotFound(new ApiResponse<FeatureDto> { Message = "Belirtilen ID'ye sahip nesne bulunamad�.", IsSuccess = false });
            }

            var response = new ApiResponse<FeatureDto> { Data = featureDto, Message = "Nesne ba�ar�yla bulundu." };
            return Ok(response);
        }

        /// <summary>
        /// Yeni bir mekansal nesne (Point, Polygon vb.) ekler.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateFeatureDto createDto)
        {
            var addedFeatureDto = await _featureService.AddAsync(createDto);

            // E�er servis null d�nerse, bu, eklenen poligonun mevcut poligonlar taraf�ndan
            // tamamen yutuldu�u ve geriye bir �ey kalmad��� anlam�na gelir.
            // Bu bir hata de�ildir. �stemciye i�lemin yap�ld���n� ama
            // yeni bir kaynak olu�turulmad���n� bildirmek i�in 204 No Content d�nebiliriz.
            if (addedFeatureDto == null)
            {
                return NoContent(); // HTTP 204
            }

            var response = new ApiResponse<FeatureDto> { Data = addedFeatureDto, Message = "Nesne ba�ar�yla eklendi." };

            return CreatedAtAction(nameof(GetById), new { id = addedFeatureDto.Id }, response);
        }

        /// <summary>
        /// Mevcut bir mekansal nesneyi g�nceller.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateFeatureDto updateDto)
        {
            var resultFeatureDto = await _featureService.UpdateAsync(id, updateDto);

            if (resultFeatureDto == null)
            {
                return NotFound(new ApiResponse<FeatureDto> { Message = "G�ncellenecek nesne bulunamad�.", IsSuccess = false });
            }

            var response = new ApiResponse<FeatureDto> { Data = resultFeatureDto, Message = "Nesne ba�ar�yla g�ncellendi." };
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
                return NotFound(new ApiResponse<object> { Message = "Silinecek nesne bulunamad�.", IsSuccess = false });
            }

            return Ok(new ApiResponse<object> { Message = "Nesne ba�ar�yla silindi." });
        }
    }
}