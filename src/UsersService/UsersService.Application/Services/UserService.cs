using AutoMapper;
using UsersService.Application.DTO;
using UsersService.Application.Interfaces.Repository;
using UsersService.Application.Interfaces.Services;
using UsersService.Domain.Entities;

namespace UsersService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUsersRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async  Task<UserDTO> GetUserByUserID(Guid userId)
        {
           User? user = await _userRepository.GetByIdAsync(userId);

           return  _mapper.Map<UserDTO>(user);

        }

        public async Task<AuthenticationResponse?> Login(LoginRequestCustom loginRequest)
        {
            User? user = await
            _userRepository.GetUserByEmailAndPassword(
                loginRequest.Email,
                loginRequest.Password
            );

            if (user == null)
            {
                return null;
            }

            // THIS CODE WAS REEMPLACE BY AUTOMMAPER
            //return new AuthenticationResponse(
            //   user.UserId,
            //   user.Password,
            //   user.PersonName,
            //   user.Gender,
            //   "token",
            //   Success: true
            //);

            return _mapper.Map<AuthenticationResponse>(user) with
            { Success = true, Token = "token" };
        }

        public async Task<AuthenticationResponse> Register(RegisterRequestCustom registerRequest)
        {
            // THIS CODE WAS REEMPLACE BY AUTOMMAPER
            //Create a new User object from RegisterRquest
            //User user = new User()
            //{
            //    PersonName = registerRequest.PersonName,
            //    Email = registerRequest.Email,
            //    Password = registerRequest.Password,
            //    Gender = registerRequest.Gender.ToString()
            //};

            User user = _mapper.Map<User>(registerRequest);

            User? registeredUser = await _userRepository.AddAsync(user);

            if (registeredUser == null)
            {
                return null;
            }

            // THIS CODE WAS REEMPLACE BY AUTOMMAPER
            //Create a new User object from RegisterRquest
            //return new AuthenticationResponse(
            //  registeredUser.UserId,
            //  registeredUser.Password,
            //  registeredUser.PersonName,
            //  registeredUser.Gender,
            //  "token",
            //  Success: true
            //);

            return _mapper.Map<AuthenticationResponse>(user) with
            { Success = true, Token = "token" };

        }
    }
}
