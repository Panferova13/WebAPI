using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lrs.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CitiesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetCities(Guid cityId)
        {
            var actionResult = checkResult(cityId);
            if (actionResult != null)
                return actionResult;
            var citiesFromDb = _repository.City.GetCities(cityId, false);
            var citiesDto = _mapper.Map<IEnumerable<CityDto>>(citiesFromDb);
            return Ok(citiesDto);
        }
        [HttpGet("{id}", Name = "GetCity")]
        public IActionResult GetCity(Guid cityId, Guid id)
        {
            var actionResult = checkResult(cityId);
            if (actionResult != null)
                return actionResult;
            var cityDb = _repository.City.GetCity(id, false);
            if (cityDb == null)
            {
                _logger.LogInfo($"City with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var city = _mapper.Map<CityDto>(cityDb);
            return Ok(city);
        }
        [HttpPost]
        public IActionResult CreateCity(Guid cityId, [FromBody] CityCreateDto city)
        {
            if (city == null)
            {
                _logger.LogError("CityCreateDto object sent from client is null.");
                return BadRequest("CityCreateDto object is null");
            }
            var actionResult = checkResult(cityId);
            if (actionResult != null)
                return actionResult;
            var cityEntity = _mapper.Map<City>(city);
            _repository.City.CreateCity(cityId, cityEntity);
            _repository.Save();
            var cityReturn = _mapper.Map<CityDto>(cityEntity);
            return CreatedAtRoute("GetCity", new
            {
                cityReturn.Id
            }, cityReturn);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteCity(Guid cityId, Guid id)
        {
            var actionResult = checkResult(cityId);
            if (actionResult != null)
                return actionResult;
            var city = _repository.City.GetCity(id, false);
            if (city == null)
            {
                _logger.LogInfo($"City with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.City.DeleteCity(city);
            _repository.Save();
            return NoContent();
        }
        [HttpPut("{id}")]
        public IActionResult UpdateCity(Guid id, [FromBody] CityUpdateDto city)
        {
            if (city == null)
            {
                _logger.LogError("CityUpdateDto object sent from client is null.");
                return BadRequest("CityUpdateDto object is null");
            }
            var actionResult = checkResult(id);
            if (actionResult != null)
                return actionResult;
            var cityEntity = _repository.City.GetCity(id, true);
            if (cityEntity == null)
            {
                _logger.LogInfo($"City with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _mapper.Map(city, cityEntity);
            _repository.Save();
            return NoContent();
        }
        private IActionResult checkResult(Guid cityId)
        {
           
            {
                _logger.LogInfo($"Country with id: {cityId} doesn't exist in the database.");
                return NotFound();
            }
            return null;
        }
    }
}
