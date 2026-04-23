using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User? GetUserById(int id);
        User CreateUser(User user);
        User? UpdateUser(int id, User user);
        bool DeleteUser(int id);
        bool UserExists(int id);
        bool EmailExists(string email, int? excludeId = null);
    }

    public class UserService : IUserService
    {
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", Age = 28, Role = "Admin", CreatedAt = DateTime.UtcNow },
            new User { Id = 2, Name = "Bob Smith", Email = "bob@example.com", Age = 34, Role = "User", CreatedAt = DateTime.UtcNow },
            new User { Id = 3, Name = "Carol White", Email = "carol@example.com", Age = 22, Role = "User", CreatedAt = DateTime.UtcNow }
        };
        private static int _nextId = 4;

        public List<User> GetAllUsers() => _users;

        public User? GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public User CreateUser(User user)
        {
            user.Id = _nextId++;
            user.CreatedAt = DateTime.UtcNow;
            _users.Add(user);
            return user;
        }

        public User? UpdateUser(int id, User updatedUser)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null) return null;

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Age = updatedUser.Age;
            user.Role = updatedUser.Role;
            return user;
        }

        public bool DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;
            _users.Remove(user);
            return true;
        }

        public bool UserExists(int id) => _users.Any(u => u.Id == id);

        public bool EmailExists(string email, int? excludeId = null) =>
            _users.Any(u => u.Email.ToLower() == email.ToLower() && u.Id != excludeId);
    }
}
