using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Application.Interfaces.Repository;
using UsersService.Domain.Entities;
using UsersService.Infrastructure.DbContext;

namespace UsersService.Infrastructure.Repositories
{
    public  class UsersRepository: IUsersRepository
    {

        private readonly GenericRepository<User> _genericRepository;
        private readonly DapperDbContext _dbContext;

        public UsersRepository(DapperDbContext dbContext)
        {
            _dbContext = dbContext;
            _genericRepository = new GenericRepository<User>(dbContext);
        }


        public async Task<User?> AddAsync(User entity)
        {
            return await _genericRepository.AddAsync(entity);
        }

        public async Task<User?> GetUserByEmailAndPassword(string? email, string? password)
        {
            //Sql query to select a user by email and password
            string query = "SELECT * FROM public.\"Users\" WHERE \"Email\" = @Email AND \"Password\" = @Password";
            var parameters = new { Email = email, Password = password };
            var user = await _dbContext.DbConnection.QueryFirstOrDefaultAsync<User>(query, parameters);

            return user;
        }
    }
}
