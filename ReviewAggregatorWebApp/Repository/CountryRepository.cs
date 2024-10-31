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
    }
}
