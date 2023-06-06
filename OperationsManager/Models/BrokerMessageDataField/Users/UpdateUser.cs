using OperationsManager.Database.Entities;

namespace OperationsManager.Models.BrokerMessageDataField.Users
{
    public class UpdateUser
    {
        public User User { get; set; }
        public string Email { get; set; }
    }
}
