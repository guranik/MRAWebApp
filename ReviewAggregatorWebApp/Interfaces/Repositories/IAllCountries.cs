﻿using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Interfaces.Repositories
{
    public interface IAllCountries
    {
        IEnumerable<Country> AllCountries { get; }
        IEnumerable<Country> SortedCountries { get; }
        int GetTotalCount();
        PagedList<Country> GetPagedCountries(int pageNumber, int pageSize);
        Country GetById(int id);
        void Create(Country country);
        void Update(Country country);
        void Delete(Country country);
    }
}
