using ReviewAggregatorWebApp.Interfaces;
using ReviewAggregatorWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAggregatorWebApp.Repository
{
    public class UserRepository : IAllUsers
    {
        private readonly Db8428Context _context;
        public UserRepository(Db8428Context context) 
        {
            _context = context;
        }

        public IEnumerable<User> AllUsers => _context.Users;
        public User GetUser(int id) => _context.Users.FirstOrDefault(x => x.Id == id)
            ?? throw new InvalidOperationException($"User with ID {id} not found.");
    }
}
