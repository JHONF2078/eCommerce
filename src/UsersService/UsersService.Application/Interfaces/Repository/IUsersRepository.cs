using UsersService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Interfaces.Repository
{
    /// <summary>
    /// Contract to be implemend by UsersRepository that  contains data access logic 
    /// of users data store
    /// </summary>
    public interface IUsersRepository : IGenericRepository<User>
    {             
        Task<User?> GetUserByEmailAndPassword(string? email, string? password);
    }
}
