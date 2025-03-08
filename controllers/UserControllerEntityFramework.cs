using AutoMapper;
using dot_net_learning_api.DTO;
using dot_net_learning_api.Model;
using Microsoft.AspNetCore.Mvc;

namespace dot_net_learning_api.controllers;

[ApiController]
[Route("[controller]")]
public class UserControllerEntityFramework : ControllerBase
{
    private IUserRepository _userRepository;

    private IMapper _mapper;

    public UserControllerEntityFramework(IUserRepository userRepository)
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserDTO, User>();
        }));
        _userRepository = userRepository;
    }

    [HttpPost("AddUser")]
    public IActionResult addUser(UserDTO user)
    {
        User newUser = _mapper.Map<User>(user);

        if (_userRepository.AddUser(newUser))
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }

    }

    [HttpPut("UpdateUser")]
    public IActionResult updateUser(User updateUser)
    {
        if (_userRepository.UpdateUser(updateUser))
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }


    [HttpGet("GetSingleUser")]
    public IActionResult getSignleUser(int userId)
    {
        User? user = _userRepository.GetUser(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }


    [HttpGet("GetUsers")]
    public IActionResult getUsers()
    {
        return Ok(_userRepository.GetUsers());
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult deleteUser(int userId)
    {
        if (_userRepository.RemoveUser(userId))
        {
            return Ok();
        }
        else
        {
            return BadRequest();

        }

    }

}

