using System.Collections.Generic;
using ProffyBackend.Models;

namespace ProffyBackend.Repositories
{
    public interface IUserRepository
    {
        public void Save(User user);

        public IEnumerable<User> FindAll();

        public User FindByEmail(string email);
    }
}