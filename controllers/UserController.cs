using dot_net_api.Data;
using DOT_NET_API.Model;
using dot_net_learning_api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace dot_net_api.controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private DataContextDapper _dataContextDapper;

    public UserController(IConfiguration configuration)
    {
        _dataContextDapper = new DataContextDapper(configuration);
    }

    [HttpPost("AddUser")]
    public IActionResult addUser(UserDTO user)
    {
        var isSaved = _dataContextDapper.SaveData<User>($"INSERT INTO DotNetCourseDatabase.TutorialAppSchema.Users (FirstName, LastName, Email, Gender, Active) VALUES ('{user.FirstName}', '{user.LastName}', '{user.Email}', '{user.Gender}','{user.Active}')");
        if (!isSaved)
        {
            return BadRequest();
        }
        return Ok();
    }

    [HttpPut("UpdateUser")]
    public IActionResult updateUser(User user)
    {
        var isUpdated = _dataContextDapper.SaveData<User>($"UPDATE DotNetCourseDatabase.TutorialAppSchema.Users SET FirstName = '{user.FirstName}', LastName = '{user.LastName}', Email = '{user.Email}' WHERE UserId = {user.UserId}");
        if (!isUpdated)
        {
            return BadRequest();
        }
        return Ok();
    }


    [HttpGet("GetSingleUser")]
    public IActionResult getSignleUser(string userId)
    {
        User user = _dataContextDapper.LoadSingleData<User>($"SELECT * FROM DotNetCourseDatabase.TutorialAppSchema.Users WHERE UserId = {userId}");

        return Ok(user);
    }


    [HttpGet("GetUsers")]
    public IActionResult getUsers()
    {
        var userList = _dataContextDapper.LoadData<User>("SELECT * FROM DotNetCourseDatabase.TutorialAppSchema.Users");
        if (userList == null)
        {
            return NotFound();
        }
        return Ok(userList);
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult deleteUser(string userId)
    {
        var isDeleted = _dataContextDapper.SaveData<User>($"DELETE FROM DotNetCourseDatabase.TutorialAppSchema.Users WHERE UserId = {userId}");
        if (!isDeleted)
        {
            return BadRequest();
        }
        return Ok();
    }

}

