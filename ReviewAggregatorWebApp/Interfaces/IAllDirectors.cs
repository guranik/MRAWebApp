using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Interfaces
{
    public interface IAllDirectors
    {
        IEnumerable<Director> AllDirectors { get; }
        int GetTotalCount();
        PagedList<Director> GetPagedDirectors(int pageNumber, int pageSize);
        Director GetById(int id);
        void Create(Director director);
        void Update(Director director);
        void Delete(Director director);
    }
}
