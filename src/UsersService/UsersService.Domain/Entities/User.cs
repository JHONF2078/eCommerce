using UsersService.Domain.Entities;

namespace UsersService.Domain.Entities
{
    /// <summary>
    /// Define the ApplicationUser class with acts as entity model class
    /// to store user details in data store
    /// </summary>
    public class User : IGenericEntity<Guid>
    {
        public Guid UserID { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PersonName { get; set; }
        public string? Gender { get; set; }

        public Guid GetId() => UserID;
        public void SetId(Guid id) => UserID = id;

        public string GetTableName() => "Users";
    }
}
