using Admin_GM.Models;
using Bogus;

namespace Admin_GM.Data
{
    public static class SeedData
    {
        public static void Initialize(EmployeeDbContext context)
        {
            if (context.Employee.Any()) return;

            var departments = new[] { "IT", "HR", "Finance", "Operations", "Marketing" };
            var categories = new[] { "Hardware", "Software", "Furniture", "Office Supplies", "Electronics", "Networking", "Vehicles", "Tools" };
            var transactionTypes = new[] { "Purchase", "Assignment", "Return" };
            var hrTypes = new[] { "Leave Request", "Performance Note", "Warning", "Promotion", "Training" };
            var hrStatuses = new[] { "Pending", "Approved", "Rejected", "Closed" };

            Randomizer.Seed = new Random(42);

            var employeeFaker = new Faker<Employee>()
                .RuleFor(e => e.Name, f => f.Name.LastName())
                .RuleFor(e => e.Prename, f => f.Name.FirstName())
                .RuleFor(e => e.CIN, f => f.Random.Replace("########"))
                .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.Prename, e.Name))
                .RuleFor(e => e.Phone_Number, f => f.Phone.PhoneNumber("+216 ## ### ###"))
                .RuleFor(e => e.Home_Address, f => f.Address.FullAddress())
                .RuleFor(e => e.Emp_Function, f => f.Name.JobTitle())
                .RuleFor(e => e.Department, f => f.PickRandom(departments));

            var employees = employeeFaker.Generate(300);
            context.Employee.AddRange(employees);
            context.SaveChanges();

            var employeeIds = context.Employee.Select(e => e.ID).ToList();

            var materialFaker = new Faker<Material>()
                .RuleFor(m => m.Name, f => f.Commerce.ProductName())
                .RuleFor(m => m.Price, f => Math.Round(f.Random.Double(50, 5000), 2))
                .RuleFor(m => m.Purchase_Date, f => f.Date.Between(new DateTime(2020, 1, 1), new DateTime(2023, 1, 31)))
                .RuleFor(m => m.Brand, f => f.Company.CompanyName())
                .RuleFor(m => m.Reference, f => f.Commerce.Ean13())
                .RuleFor(m => m.IsTaken, f => f.Random.Int(0, 1))
                .RuleFor(m => m.Category, f => f.PickRandom(categories));

            var materials = materialFaker.Generate(500);
            context.Material.AddRange(materials);
            context.SaveChanges();

            var materialIds = context.Material.Select(m => m.ID).ToList();

            var assignFaker = new Faker<Assign>()
                .RuleFor(a => a.ID_Employee, f => f.PickRandom(employeeIds))
                .RuleFor(a => a.ID_Material, f => f.PickRandom(materialIds))
                .RuleFor(a => a.Start_Date, f => f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2022, 6, 30)))
                .RuleFor(a => a.End_Date, (f, a) => a.Start_Date.AddDays(f.Random.Int(30, 365)))
                .RuleFor(a => a.TransactionType, f => f.PickRandom(transactionTypes));

            var assigns = assignFaker.Generate(200);
            context.Assign.AddRange(assigns);
            context.SaveChanges();

            var hrFaker = new Faker<HRRecord>()
                .RuleFor(h => h.ID_Employee, f => f.PickRandom(employeeIds))
                .RuleFor(h => h.Type, f => f.PickRandom(hrTypes))
                .RuleFor(h => h.Description, f => f.Lorem.Sentence(10))
                .RuleFor(h => h.Date, f => f.Date.Between(new DateTime(2022, 1, 1), new DateTime(2023, 1, 31)))
                .RuleFor(h => h.Status, f => f.PickRandom(hrStatuses));

            var hrRecords = hrFaker.Generate(50);
            context.HRRecord.AddRange(hrRecords);
            context.SaveChanges();
        }
    }
}
