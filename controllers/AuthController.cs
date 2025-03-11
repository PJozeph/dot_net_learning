using System.Data;
using System.Security.Cryptography;
using dot_net_api.Data;
using dot_net_learning_api.DTO;
using dot_net_learning_api.Model;
using dot_net_learning_api.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace dot_net_learning_api.controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly DataContextDapper _dapper;
        private IConfiguration _config;
        private AuthUtils _authUtils;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
            _authUtils = new AuthUtils(config);
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public IActionResult Register(UserForRegistrationDto userRegistrationDto)
        {
            if (userRegistrationDto.Password == userRegistrationDto.PasswordConfirm)
            {
                string sqlCheckUserPresent = $"SELECT * FROM DotNetCourseDatabase.TutorialAppSchema.Users WHERE Email = '{userRegistrationDto.Email}'";
                var user = _dapper.LoadSingleData<User>(sqlCheckUserPresent);
                if (user == null)
                {
                    byte[] randomSalt = new byte[128 / 8];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(randomSalt);
                    }

                    byte[] passwordHash = _authUtils.GetPasswordHash(userRegistrationDto.Password, randomSalt);
                    string sqlAddUser = $@"
                        INSERT INTO DotNetCourseDatabase.TutorialAppSchema.Auth 
                        (Email, PasswordHash, PasswordSalt) 
                        VALUES 
                        ('{userRegistrationDto.Email}', @PasswordHash, @PasswordSalt)
                    ";

                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter passwordHasParam = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHasParam.Value = passwordHash;
                    SqlParameter passwordSaltParam = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParam.Value = randomSalt;

                    parameters.Add(passwordHasParam);
                    parameters.Add(passwordSaltParam);

                    bool extecuted = _dapper.executeSqlWithParams<User>(sqlAddUser, parameters);
                    if (extecuted)
                    {
                        var isSaved = _dapper.SaveData<User>($"INSERT INTO DotNetCourseDatabase.TutorialAppSchema.Users (FirstName, LastName, Email, Gender, Active) VALUES ('{userRegistrationDto.FirstName}', '{userRegistrationDto.LastName}', '{userRegistrationDto.Email}', '{userRegistrationDto.Gender}','{1}')");
                        if (isSaved)
                        {
                            User savedUser = _dapper.LoadSingleData<User>($"SELECT * FROM DotNetCourseDatabase.TutorialAppSchema.Users WHERE Email = '{userRegistrationDto.Email}'");
                            string token = _authUtils.CreateToken(savedUser.UserId);
                            if (token != null)
                            {
                                return Ok(
                                    new Dictionary<string, string>
                                    {
                                        { "token", token },
                                    }
                                );
                            }
                            throw new Exception("Token not created");
                        }
                        throw new Exception("User faild to created");
                    }
                    throw new Exception("User not added");
                }
                throw new Exception("User already exists");
            }
            throw new Exception("Passwords do not match");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto user)
        {
            string sqlCheckUserPresent = $"SELECT * FROM DotNetCourseDatabase.TutorialAppSchema.Auth WHERE Email = '{user.Email}'";
            UserForLoginConfirmationDto userConfirmation = _dapper.LoadSingleData<UserForLoginConfirmationDto>(sqlCheckUserPresent);

            byte[] passwordHash = _authUtils.GetPasswordHash(user.Password, userConfirmation.PasswordSalt);

            if (passwordHash.SequenceEqual(userConfirmation.PasswordHash))
            {
                string userIDSql = $"SELECT UserId FROM DotNetCourseDatabase.TutorialAppSchema.Users WHERE Email = '{user.Email}'";
                int userFromDb = _dapper.LoadSingleData<int>(userIDSql);
                return Ok(
                    new Dictionary<string, string>
                    {
                        { "userId", _authUtils.CreateToken(userFromDb) },
                    }
                );
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";
            string userIDSql = $"SELECT UserId FROM DotNetCourseDatabase.TutorialAppSchema.Users WHERE UserId = {userId}";
            int userFromDb = _dapper.LoadSingleData<int>(userIDSql);

            return Ok(
                new Dictionary<string, string>
                {
                    { "userId", _authUtils.CreateToken(userFromDb) },
                }
            );
        }
    }

}