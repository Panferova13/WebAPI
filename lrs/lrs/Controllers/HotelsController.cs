using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace lrs.Controllers
{
    [Route("api/cities/{cityId}/hotels")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public HotelsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetHotelsAsync(Guid cityId)
        {
            var actionResult = await checkResultAsync(cityId);
            if (actionResult != null)
                return actionResult;
            var hotelsFromDb = await _repository.Hotel.GetHotelsAsync(cityId, false);
            var hotelsDto = _mapper.Map<IEnumerable<HotelDto>>(hotelsFromDb);
            return Ok(hotelsDto);
        }
        [HttpGet("{id}", Name = "GetHotel")]
        public async Task<IActionResult> GetHotelAsync(Guid cityId, Guid id)
        {
            var actionResult = await checkResultAsync(cityId);
            if (actionResult != null)
                return actionResult;
            var hotelDb = await _repository.Hotel.GetHotelAsync(cityId, id, false);
            if (hotelDb == null)
            {
                _logger.LogInfo($"Hotel with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var hotel = _mapper.Map<HotelDto>(hotelDb);
            return Ok(hotel);
        }
        [HttpPost]
        public async Task<IActionResult> GetHotelAsync(Guid cityId, [FromBody] HotelCreateDto hotel)
        {
            if (hotel == null)
            {
                _logger.LogError("HotelCreateDto object sent from client is null.");
                return BadRequest("HotelCreateDto object is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the HotelCreateDto object");
                return UnprocessableEntity(ModelState);
            }
            var actionResult = await checkResultAsync(cityId);
            if (actionResult != null)
                return actionResult;
            var hotelEntity = _mapper.Map<Hotel>(hotel);
            _repository.Hotel.CreateHotel(cityId, hotelEntity);
            _repository.SaveAsync();
            var hotelReturn = _mapper.Map<HotelDto>(hotelEntity);
            return CreatedAtRoute("GetHotel", new
            {
                cityId,
                hotelReturn.Id
            }, hotelReturn);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelAsync(Guid cityId, Guid id)
        {
            var actionResult = await checkResultAsync(cityId);
            if (actionResult != null)
                return actionResult;
            var hotel = await _repository.Hotel.GetHotelAsync(cityId, id, false);
            if (hotel == null)
            {
                _logger.LogInfo($"Hotel with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.Hotel.DeleteHotel(hotel);
            _repository.SaveAsync();
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(Guid cityId, Guid id, [FromBody] HotelUpdateDto hotel)
        {
            if (hotel == null)
            {
                _logger.LogError("HotelUpdateDto object sent from client is null.");
                return BadRequest("HotelUpdateDto object is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the HotelUpdateDto object");
                return UnprocessableEntity(ModelState);
            }
            var actionResult = await checkResultAsync(cityId);
            if (actionResult != null)
                return actionResult;
            var hotelEntity = await _repository.Hotel.GetHotelAsync(cityId, id, true);
            if (hotelEntity == null)
            {
                _logger.LogInfo($"Hotel with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _mapper.Map(hotel, hotelEntity);
            _repository.SaveAsync();
            return NoContent();
        }
        private async Task<IActionResult> checkResultAsync(Guid cityId)
        {

            var city = await _repository.City.GetCityAsync(cityId, cityId, false);
            if (city == null)
            {
                _logger.LogInfo($"City with id: {cityId} doesn't exist in the database.");
                return NotFound();
            }
            return null;
        }
    }
}
