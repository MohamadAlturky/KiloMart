namespace KiloMart.Authentication.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
}






//using dapper and sql server
//write the UserService that has this functions
//Task<int> AddAsync (IDbConnection, User user)
//Task UpdateAsync (IDbConnection, User user)
//Task DeleteAsync (IDbConnection, User user)
//Task<User> GetAsync (IDbConnection, int userId)
//Task<Role> GetRolesAsync (IDbConnection, int userId)

//and the same for the RoleService and UserRoleService


//public class User
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public string HashedPassword { get; set; }
//    public bool IsActive { get; set; }
//}

//public class Role
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//}


//public class UserRole
//{
//    public int Id { get; set; }
//    public int User { get; set; }
//    public int Role { get; set; }
//}
