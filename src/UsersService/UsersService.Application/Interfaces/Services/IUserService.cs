using UsersService.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Interfaces.Services
{
    /// <summary>
    /// Contract for users service that contains use cases for users
    /// </summary>
    public  interface IUserService
    {
        /// <summary>
        /// Method to handle user login use case and returns an AuthenticationResponse
        /// that contains status of the login
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        Task<AuthenticationResponse> Login(
          LoginRequestCustom loginRequest
        );

        /// <summary>
        /// Method to handle user registration use case and returns an object of  AuthenticationResponse
        /// type that represents status  of user registration
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns></returns>
        Task<AuthenticationResponse> Register(
            RegisterRequestCustom registerRequest
        );
    }
}
