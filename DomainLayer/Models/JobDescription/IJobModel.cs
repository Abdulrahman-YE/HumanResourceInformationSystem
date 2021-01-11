namespace DomainLayer.Models.JobDescription
{
    interface IJobModel
    {
        int EmployeeID { get; set; }
        int ID { get; set; }
        bool IsActive { get; set; }
        string JobTitle { get; set; }
        string Position { get; set; }
    }
}