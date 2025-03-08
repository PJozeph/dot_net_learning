using DOT_NET_API.Model;

namespace dot_net_api.controllers {
    public interface IUserRepository {
        bool AddUser(User user);
        bool RemoveUser(int userId);
        IEnumerable<User> GetUsers();
        User GetUser(int userId);
        bool UpdateUser(User user);
    }
}