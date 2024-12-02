using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Repository
{
    public class CountryRepository : IAllCountries
    {
        private readonly Db8428Context _context;
        public CountryRepository(Db8428Context context)
        {
            _context = context;
        }

        public IEnumerable<Country> AllCountries => _context.Countries;

        public Country GetById(int id) => _context.Countries.Find(id)
            ?? throw new InvalidOperationException($"Country with ID {id} not found.");

        public void Create(Country country)
        {
            _context.Countries.Add(country);
            _context.SaveChanges();
        }

        public void Update(Country country)
        {
            _context.Countries.Update(country);
            _context.SaveChanges();
        }

        public void Delete(Country country)
        {
            _context.Countries.Remove(country);
            _context.SaveChanges();
        }
    }
}
