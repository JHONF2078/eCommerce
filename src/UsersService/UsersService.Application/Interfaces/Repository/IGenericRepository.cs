using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Interfaces.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> AddAsync(T entity);
    }
}
