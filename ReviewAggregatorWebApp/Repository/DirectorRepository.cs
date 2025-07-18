﻿using Microsoft.EntityFrameworkCore;
using ReviewAggregatorWebApp.Interfaces.Repositories;
using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Repository
{
    public class DirectorRepository : IAllDirectors
    {
        private readonly Db8428Context _context;
        public DirectorRepository(Db8428Context context)
        {
            _context = context;
        }

        public IEnumerable<Director> AllDirectors => _context.Directors;
        public IEnumerable<Director> SortedDirectors => _context.Directors.OrderBy(c => c.Name);


        public int GetTotalCount() => _context.Directors.Count();
        public PagedList<Director> GetPagedDirectors(int pageNumber, int pageSize)
        {
            var directors = _context.Directors
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = _context.Directors.Count();
            return new PagedList<Director>(directors, totalCount, pageNumber, pageSize);
        }

        public IEnumerable<Director> GetDirectorsByNamePrefix(string namePrefix)
        {
            if (string.IsNullOrWhiteSpace(namePrefix))
            {
                return null;
            }

            return _context.Directors
                .Where(m => m.Name.StartsWith(namePrefix.ToLower()))
                .Take(8)
                .ToList();
        }

        public Director GetById(int id) => _context.Directors.Find(id)
            ?? throw new InvalidOperationException($"Director with ID {id} not found.");

        public void Create(Director director)
        {
            _context.Directors.Add(director);
            _context.SaveChanges();
        }

        public void Update(Director director)
        {
            _context.Directors.Update(director);
            _context.SaveChanges();
        }

        public void Delete(Director director)
        {
            _context.Directors.Remove(director);
            _context.SaveChanges();
        }
    }
}
