namespace DomainLayer.Models.Account
{
    interface IAccountModel
    {
        int EmployeeID { get; set; }
        int ID { get; set; }
        string Password { get; set; }
        int RoleID { get; set; }
        string Username { get; set; }
    }
}