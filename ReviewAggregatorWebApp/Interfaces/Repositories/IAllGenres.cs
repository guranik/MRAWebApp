﻿using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Interfaces.Repositories
{
    public interface IAllGenres
    {
        IEnumerable<Genre> AllGenres { get; }
        IEnumerable<Genre> SortedGenres { get; }
        Genre GetById(int id);
        void Create(Genre genre);
        void Update(Genre genre);
        void Delete(Genre genre);
    }
}
