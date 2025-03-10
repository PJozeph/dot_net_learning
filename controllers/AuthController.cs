using System.Data;
using System.Security.Cryptography;
using System.Text;
using dot_net_api.Data;
using dot_net_learning_api.DTO;
using dot_net_learning_api.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace dot_net_learning_api.controllers
{

    public class AuthController : ControllerBase
    {

        private readonly DataContextDapper _dapper;
        private IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
        }

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

                    byte[] passwordHash = GetPasswordHash(userRegistrationDto.Password, randomSalt);

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
                            return Ok();
                        }
                        throw new Exception("User faild to created");
                    }
                    throw new Exception("User not added");
                }
                throw new Exception("User already exists");
            }
            throw new Exception("Passwords do not match");
        }

        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto user)
        {

            string sqlCheckUserPresent = $"SELECT * FROM DotNetCourseDatabase.TutorialAppSchema.Auth WHERE Email = '{user.Email}'";
            UserForLoginConfirmationDto userConfirmation = _dapper.LoadSingleData<UserForLoginConfirmationDto>(sqlCheckUserPresent);

            byte[] passwordHash = GetPasswordHash(user.Password, userConfirmation.PasswordSalt);

            if (passwordHash.SequenceEqual(userConfirmation.PasswordHash))
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }

        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }
    }

}