using OperationsApi.Database.Entities;

namespace OperationsApi.Models.BrokerMessageDataField.Users
{
    public class UpdateUser
    {
        public User User { get; set; }
        public string Email { get; set; }
    }
}
