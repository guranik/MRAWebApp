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
        Director GetById(int id);
        void Create(Director director);
        void Update(Director director);
        void Delete(int id);
    }
}
