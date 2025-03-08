using dot_net_learning_api.controllers;
using dot_net_learning_api.Data;
using dot_net_learning_api.Model;

namespace dot_net_api.controllers{

    public class UserRepository : IUserRepository
    {

        private DataContextEntityFramework _dataContextEntityFramework;

        public UserRepository(IConfiguration configuration)
        {
            _dataContextEntityFramework = new DataContextEntityFramework(configuration);
        }

        public bool AddUser(User user)
        {
            _dataContextEntityFramework.Add(user);
            return _dataContextEntityFramework.SaveChanges() > 0;
        }

        public bool RemoveUser(int userId)
        {
            User? user = _dataContextEntityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();
            if (user != null)
            {
                _dataContextEntityFramework.Remove(user);
                return _dataContextEntityFramework.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateUser(User user)
        {
            User? presentUser = _dataContextEntityFramework.Users.Where(u => u.UserId == user.UserId).FirstOrDefault<User>();

            if (user != null)
            {
                presentUser.FirstName = user.FirstName;
                presentUser.LastName = user.LastName;
                presentUser.Email = user.Email;
                presentUser.Gender = user.Gender;
                presentUser.Active = user.Active;
                if (_dataContextEntityFramework.SaveChanges() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            return false;

        }

        public IEnumerable<User> GetUsers()
        {
            return _dataContextEntityFramework.Users.ToList();
        }

        public User GetUser(int userId)
        {
            return _dataContextEntityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();
        }


    }
}