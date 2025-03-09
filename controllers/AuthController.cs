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
                    byte[] passwordSalt = new byte[128 / 8];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(passwordSalt);
                    }
                    string passwordPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);
                    byte[] passwordHash = KeyDerivation.Pbkdf2(
                        password: userRegistrationDto.Password,
                        salt: Encoding.ASCII.GetBytes(passwordPlusString),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    );

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
                    passwordSaltParam.Value = passwordSalt;

                    parameters.Add(passwordHasParam);
                    parameters.Add(passwordSaltParam);

                    bool extecuted = _dapper.executeSqlWithParams<User>(sqlAddUser, parameters);
                    if (extecuted)
                    {
                        return Ok();

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
            return Ok();

        }
    }

}