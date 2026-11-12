public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Department { get; set; } = "";
    public string JobTitle { get; set; } = "";
    public decimal MonthlySalary { get; set; }
    public double TaxRate { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public string Email { get; set; } = "";
}