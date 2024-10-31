using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Interfaces
{
    public interface IAllUsers
    {
        IEnumerable<User> AllUsers { get; }
        User GetUser(int id);
    }
}
