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
            ?? throw new InvalidOperationException($"User  with ID {id} not found.");

        public void Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = GetUser(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
