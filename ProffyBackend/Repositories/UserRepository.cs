using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProffyBackend.Data;
using ProffyBackend.Data.Transactions;
using ProffyBackend.Models;

namespace ProffyBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly UnitOfWork _uow;
        
        public UserRepository([FromServices] DataContext context, [FromServices] UnitOfWork uow)
        {
            _context = context;
            _uow = uow;
        }

       public void Save(User user)
       {
           try
           {
               _context.Add(user);
               _uow.Commit();
           }
           catch (Exception)
           {
               _uow.Rollback();
           }
       }

       public IEnumerable<User> FindAll()
       {
           return _context.Users.ToList();
       }

       public User FindByEmail(string email)
       {
           return _context.Users.First(u => u.Email == email);
       }
    }
}