using System;

namespace API.DTO;

public class UserLoginDTO
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
