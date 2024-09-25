using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(UserRegisterDTO registerDTO)
    {
        if(registerDTO.UserName == "" || registerDTO.Password == "")
        {
            return BadRequest("Username or Password can't be empty");
        }
        else if(await UserExists(registerDTO.UserName))
        {
            return BadRequest("Username is taken");
        }else if(registerDTO.UserName.Contains(' ') || registerDTO.Password.Contains(' '))
        {
            return BadRequest("Username or Password can't contain spaces");
        }
        var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = registerDTO.UserName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
        
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        return new UserDTO
        {
            UserName = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(UserLoginDTO loginDTO)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.UserName.ToLower() == loginDTO.UserName.ToLower());
        if(user == null)
        {
            return Unauthorized("Username is invalid");
        }

        var hmac = new HMACSHA512(user.PasswordSalt);
        var pwdhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        if(pwdhash.SequenceEqual(user.PasswordHash))
        {
            return new UserDTO
            {
                UserName = user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }else{
            return Unauthorized("Invalid password");
        }
    }

    private async Task<bool> UserExists(string username)
    {
       return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
