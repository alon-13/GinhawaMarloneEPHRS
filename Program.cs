using System.Globalization;

List<Employee> employees = new();   // starts empty (0 records)
int nextId = 1001;
const string SystemTitle = "EPHRS - Final Merged Edition";

while (true)
{
    Console.WriteLine($"\n===== {SystemTitle} =====");
    Console.WriteLine("1. Create   2. Read   3. Search/Filter   4. Update   5. Delete   0. Exit");
    Console.Write("Choose: ");
    switch (Console.ReadLine())
    {
        case "1": Create(); break;
        case "2": PrintTable(employees.OrderBy(e => e.Id)); break;
        case "3": Search(); break;
        case "4": Update(); break;
        case "5": Delete(); break;
        case "0": return;
        default: Console.WriteLine("Invalid choice."); break;
    }
}

// ---------- CREATE ----------
void Create()
{
    var emp = new Employee
    {
        Id = nextId++,
        FirstName = Prompt("First name: "),
        LastName = Prompt("Last name: "),
        Department = Prompt("Department: "),
        JobTitle = Prompt("Job title: "),
        MonthlySalary = ReadDecimal("Monthly salary: "),
        TaxRate = ReadDouble("Tax rate (e.g. 0.12): "),
        HireDate = ReadDate("Hire date (yyyy-MM-dd): "),
        IsActive = ReadBool("Active? (y/n): "),
        Email = Prompt("Email: ")
    };
    employees.Add(emp);
    Console.WriteLine($"Created employee {emp.Id}.");
}

// ---------- SEARCH / FILTER (LINQ) ----------
void Search()
{
    Console.WriteLine("1. By name  2. By department  3. Active only  4. Salary >= amount  5. Hired after date");
    Console.Write("Choose: ");
    IEnumerable<Employee> result = Console.ReadLine() switch
    {
        "1" => SearchName(Prompt("Name contains: ")),
        "2" => employees.Where(e => e.Department.Equals(Prompt("Department: "), StringComparison.OrdinalIgnoreCase)),
        "3" => employees.Where(e => e.IsActive),
        "4" => FilterSalary(ReadDecimal("Minimum salary: ")),
        "5" => FilterDate(ReadDate("Date (yyyy-MM-dd): ")),
        _ => Enumerable.Empty<Employee>()
    };
    PrintTable(result);
}
IEnumerable<Employee> SearchName(string t) =>
    employees.Where(e => e.FirstName.Contains(t, StringComparison.OrdinalIgnoreCase)
                      || e.LastName.Contains(t, StringComparison.OrdinalIgnoreCase));
IEnumerable<Employee> FilterSalary(decimal min) => employees.Where(e => e.MonthlySalary >= min);
IEnumerable<Employee> FilterDate(DateTime d) => employees.Where(e => e.HireDate > d);

// ---------- UPDATE ----------
void Update()
{
    int id = (int)ReadDecimal("Employee Id to update: ");
    var emp = employees.FirstOrDefault(e => e.Id == id);
    if (emp == null) { Console.WriteLine("Not found."); return; }

    Console.WriteLine("Press Enter to keep the current value.");
    emp.FirstName = PromptKeep("First name", emp.FirstName);
    emp.LastName = PromptKeep("Last name", emp.LastName);
    emp.Department = PromptKeep("Department", emp.Department);
    emp.JobTitle = PromptKeep("Job title", emp.JobTitle);
    emp.Email = PromptKeep("Email", emp.Email);

    var sal = PromptKeep("Monthly salary", emp.MonthlySalary.ToString());
    if (decimal.TryParse(sal, out var s)) emp.MonthlySalary = s;
    var tax = PromptKeep("Tax rate", emp.TaxRate.ToString());
    if (double.TryParse(tax, out var t)) emp.TaxRate = t;
    var act = PromptKeep("Active (y/n)", emp.IsActive ? "y" : "n");
    emp.IsActive = act.StartsWith("y", StringComparison.OrdinalIgnoreCase);
    Console.WriteLine("Updated.");
}

// ---------- DELETE ----------
void Delete()
{
    int id = (int)ReadDecimal("Employee Id to delete: ");
    var emp = employees.FirstOrDefault(e => e.Id == id);
    if (emp == null) { Console.WriteLine("Not found."); return; }
    employees.Remove(emp);
    Console.WriteLine($"Deleted {emp.FirstName} {emp.LastName}.");
}

// ---------- DISPLAY ----------
void PrintTable(IEnumerable<Employee> list)
{
    var rows = list.ToList();
    if (rows.Count == 0) { Console.WriteLine("No records."); return; }
    Console.WriteLine($"{"Id",-6}{"First",-10}{"Last",-10}{"Dept",-9}{"Title",-18}{"Salary",12}{"Tax",6}{"Hired",12}{"Active",8}  Email");
    foreach (var e in rows)
        Console.WriteLine($"{e.Id,-6}{e.FirstName,-10}{e.LastName,-10}{e.Department,-9}{e.JobTitle,-18}{e.MonthlySalary,12:N2}{e.TaxRate,6:P0}{e.HireDate,12:yyyy-MM-dd}{e.IsActive,8}  {e.Email}");
    Console.WriteLine($"Total: {rows.Count} | Total payroll: {rows.Sum(e => e.MonthlySalary):N2} | Avg salary: {rows.Average(e => e.MonthlySalary):N2}");
}

// ---------- INPUT HELPERS ----------
string Prompt(string label) { Console.Write(label); return Console.ReadLine() ?? ""; }
string PromptKeep(string label, string current)
{
    Console.Write($"{label} [{current}]: ");
    var v = Console.ReadLine();
    return string.IsNullOrWhiteSpace(v) ? current : v;
}
decimal ReadDecimal(string label)
{
    while (true) { Console.Write(label); if (decimal.TryParse(Console.ReadLine(), out var v)) return v; Console.WriteLine("Invalid number."); }
}
double ReadDouble(string label)
{
    while (true) { Console.Write(label); if (double.TryParse(Console.ReadLine(), out var v)) return v; Console.WriteLine("Invalid number."); }
}
DateTime ReadDate(string label)
{
    while (true) { Console.Write(label); if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var v)) return v; Console.WriteLine("Use yyyy-MM-dd."); }
}
bool ReadBool(string label)
{
    while (true)
    {
        Console.Write(label);
        var v = (Console.ReadLine() ?? "").Trim().ToLower();
        if (v is "y" or "yes" or "true") return true;
        if (v is "n" or "no" or "false") return false;
        Console.WriteLine("Enter y or n.");
    }
}