﻿using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using lrs.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private readonly IDataShaper<HotelDto> _dataShaper;
        public HotelsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<HotelDto> dataShaper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _dataShaper = dataShaper;
        }
        /// <summary>
        /// Возвращает список отелей города
        /// </summary>
        /// <param name="partWorldId"></param>
        /// <param name="countryId"></param>
        /// <param name="cityId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetHotelsAsync(Guid partWorldId, Guid countryId, Guid cityId, [FromQuery] HotelParameters parameters)
        {
            var actionResult = await checkResultAsync(partWorldId, countryId, cityId);
            if (actionResult != null)
                return actionResult;
            var hotelsFromDb = await _repository.Hotel.GetHotelsAsync(cityId, parameters, false);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(hotelsFromDb.MetaData));
            var hotelsDto = _mapper.Map<IEnumerable<HotelDto>>(hotelsFromDb);
            return Ok(_dataShaper.ShapeData(hotelsDto, parameters.Fields));
        }
        /// <summary>
        /// Возвращает отель города
        /// </summary>
        /// <param name="partWorldId"></param>
        /// <param name="countryId"></param>
        /// <param name="cityId"></param>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetHotel")]
        [HttpHead("{id}")]
        public async Task<IActionResult> GetHotelAsync(Guid partWorldId, Guid countryId, Guid cityId, Guid id, [FromQuery] HotelParameters parameters)
        {
            var actionResult = await checkResultAsync(partWorldId, countryId, cityId);
            if (actionResult != null)
                return actionResult;
            var hotelDb = await _repository.Hotel.GetHotelAsync(cityId, id, false);
            if (hotelDb == null)
            {
                _logger.LogInfo($"Hotel with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var hotelDto = _mapper.Map<HotelDto>(hotelDb);
            return Ok(_dataShaper.ShapeData(hotelDto, parameters.Fields));
        }
        /// <summary>
        /// Создает отель
        /// </summary>
        /// <param name="partWorldId"></param>
        /// <param name="countryId"></param>
        /// <param name="cityId"></param>
        /// <param name="hotel"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> GetHotelAsync(Guid partWorldId, Guid countryId, Guid cityId, [FromBody] HotelCreateDto hotel)
        {
            var actionResult = await checkResultAsync(partWorldId, countryId, cityId);
            if (actionResult != null)
                return actionResult;
            var hotelEntity = _mapper.Map<Hotel>(hotel);
            _repository.Hotel.CreateHotel(cityId, hotelEntity);
            await _repository.SaveAsync();
            var hotelReturn = _mapper.Map<HotelDto>(hotelEntity);
            return CreatedAtRoute("GetHotel", new
            {
                partWorldId,
                countryId,
                cityId,
                hotelReturn.Id
            }, hotelReturn);
        }
        /// <summary>
        /// Удаляет отель
        /// </summary>
        /// <param name="partWorldId"></param>
        /// <param name="countryId"></param>
        /// <param name="cityId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelAsync(Guid partWorldId, Guid countryId, Guid cityId, Guid id)
        {
            var actionResult = await checkResultAsync(partWorldId, countryId, cityId);
            if (actionResult != null)
                return actionResult;
            var hotel = await _repository.Hotel.GetHotelAsync(cityId, id, false);
            if (hotel == null)
            {
                _logger.LogInfo($"Hotel with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.Hotel.DeleteHotel(hotel);
            await _repository.SaveAsync();
            return NoContent();
        }
        /// <summary>
        /// Обновляет отель
        /// </summary>
        /// <param name="partWorldId"></param>
        /// <param name="countryId"></param>
        /// <param name="cityId"></param>
        /// <param name="id"></param>
        /// <param name="hotel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateHotelExistsAttribute))]
        public async Task<IActionResult> UpdateHotel(Guid partWorldId, Guid countryId, Guid cityId, Guid id, [FromBody] HotelUpdateDto hotel)
        {
            var hotelEntity = HttpContext.Items["hotel"] as Hotel;
            _mapper.Map(hotel, hotelEntity);
            await _repository.SaveAsync();
            return NoContent();
        }
        private async Task<IActionResult> checkResultAsync(Guid partWorldId, Guid countryId, Guid cityId)
        {
            var partWorld = await _repository.PartWorld.GetPartWorldAsync(partWorldId, false);
            if (partWorld == null)
            {
                _logger.LogInfo($"Partworld with id: {partWorldId} doesn't exist in the database.");
                return NotFound();
            }
            var country = await _repository.Country.GetCountryAsync(partWorldId, countryId, false);
            if (country == null)
            {
                _logger.LogInfo($"Country with id: {countryId} doesn't exist in the database.");
                return NotFound();
            }
            var city = await _repository.City.GetCityAsync(countryId, cityId, false);
            if (city == null)
            {
                _logger.LogInfo($"City with id: {countryId} doesn't exist in the database.");
                return NotFound();
            }
            return null;
        }
        /// <summary>
        /// Возвращает заголовки запросов
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, DELETE, PUT, PATCH");
            return Ok();
        }
    }
}
