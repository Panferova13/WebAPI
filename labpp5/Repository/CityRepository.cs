using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CityRepository : RepositoryBase<City>, ICityRepository
    {
        public CityRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateCity(Guid cityId, City city)
        {
            Create(city);
        }

        public IEnumerable<City> GetCities(Guid countryId, bool trackChanges) =>
            FindAll(trackChanges).OrderBy(c => c.Name).ToList();

        public City GetCity(Guid cityId, Guid id, bool trackChanges) =>
            FindByCondition(c => c.Id.Equals(cityId), trackChanges).SingleOrDefault();
    }
}
