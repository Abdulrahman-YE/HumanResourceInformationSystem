namespace DomainLayer.Models.Account
{
    public interface IAccountModel
    {
        int EmployeeID { get; set; }
        int ID { get; set; }
        string Password { get; set; }
        int RoleID { get; set; }
        string Username { get; set; }
    }
}