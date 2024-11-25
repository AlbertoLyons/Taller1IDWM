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
                Mail = userModel.Email!,
                Gender = userModel.Gender,
                Birthdate = userModel.Birthdate,
                Password = userModel.PasswordHash!
            };
        }
        public static UserGetAdminDTO ToUserGetAdminDTO(this User userModel)
        {
            return new UserGetAdminDTO
            {
                Id = userModel.Id,
                Rut = userModel.Rut,
                Name = userModel.Name,
                Mail = userModel.Email!,
                Gender = userModel.Gender,
                Birthdate = userModel.Birthdate,
            };
        }
        public static User ToUserFromUpdateDTO(this UpdateUserDTO updateUserDto)
        {
            return new User
            {
                Name = updateUserDto.Name,
                Gender = updateUserDto.Gender,
                Birthdate = updateUserDto.Birthdate,
                PasswordHash = updateUserDto.Password
            };
        }
        public static UserNameDTO ToUserNameDTO(this User userModel)
        {
            return new UserNameDTO
            {
                id = userModel.Id,
                name = userModel.Name
            };
        }
    }
}