using Taller_1_IDWM.src.DTOs.Auth;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Mappers
{
    public static class AuthMapper
    {
        public static User ToUserFromRegisteredDTO(this RegisterDTO registerDTO)
        {
            return new User
            {
                Rut = registerDTO.Rut,
                Name = registerDTO.Name,
                Email = registerDTO.Mail,
                UserName = registerDTO.Mail,
                Gender = registerDTO.Gender,
                Birthdate = registerDTO.Birthdate,
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }
        public static NewUserDTO ToNewUserDTO(this User userModel, string token)
        {
            return new NewUserDTO
            {
                Username = userModel.UserName!,
                Email = userModel.Email!,
                Token = token
            };
        }
    }
}