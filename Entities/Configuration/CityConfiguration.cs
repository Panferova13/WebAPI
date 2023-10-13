using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Configuration
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasData
            (
                 new City
                 {
                     Id = new Guid("8daf4fdc-310b-4b7d-acf4-2f5291b47000"),
                     Name = "Анапа",
                 },
                 new City
                 {
                     Id = new Guid("8daf4fdc-310b-4b7d-acf4-2f5291b47001"),
                     Name = "Мадрид",
                 },
                 new City
                 {
                     Id = new Guid("8daf4fdc-310b-4b7d-acf4-2f5291b47002"),
                     Name = "Саранск",
                 }
            );
        }
    }
}
