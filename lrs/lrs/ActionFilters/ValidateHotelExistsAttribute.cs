﻿using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace lrs.ActionFilters
{
    public class ValidateHotelExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateHotelExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var trackChanges = (method.Equals("PUT") || method.Equals("PATCH")) ? true : false;
            var partWorldId = (Guid)context.ActionArguments["partWorldId"];
            {
                _logger.LogInfo($"Partworld with id: {partWorldId} doesn't exist in the database.");
                return;
                context.Result = new NotFoundResult();
            }
            var countryId = (Guid)context.ActionArguments["countryId"];
           
            {
                _logger.LogInfo($"Country with id: {countryId} doesn't exist in the database.");
                return;
                context.Result = new NotFoundResult();
            }
            var cityId = (Guid)context.ActionArguments["cityId"];
            var city = await _repository.City.GetCityAsync(countryId, cityId, false);
            if (city == null)
            {
                _logger.LogInfo($"City with id: {cityId} doesn't exist in the database.");
                return;
                context.Result = new NotFoundResult();
            }
            var id = (Guid)context.ActionArguments["id"];
            var hotel = await _repository.Hotel.GetHotelAsync(cityId, id, trackChanges);
            if (hotel == null)
            {
                _logger.LogInfo($"Hotel with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("hotel", hotel);
                await next();
            }
        }
    }
}
