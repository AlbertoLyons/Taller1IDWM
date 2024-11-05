using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Users;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Mappers
{
    public static class UserMapper
    {
        public static UserDTO ToUserDTO(this User userModel)
        {
            return new UserDTO
            {
                Id = userModel.Id,
                Rut = userModel.Rut,
                Name = userModel.Name,
                Mail = userModel.Mail,
                Gender = userModel.Gender,
                Birthdate = userModel.Birthdate,
                Password = userModel.Password
            };
        }

        public static User ToUserFromCreatedDTO(this CreateUserDTO createUserDto)
        {
            return new User
            {
                Rut = createUserDto.Rut,
                Name = createUserDto.Name,
                Mail = createUserDto.Mail,
                Gender = createUserDto.Gender,
                Birthdate = createUserDto.Birthdate,
                Password = createUserDto.Password
            };
        }
        public static User ToUserFromUpdateDTO(this CreateUserDTO createUserDto)
        {
            return new User
            {
                Name = createUserDto.Name,
                Gender = createUserDto.Gender,
                Birthdate = createUserDto.Birthdate,
                Password = createUserDto.Password
            };
        }
    }
}