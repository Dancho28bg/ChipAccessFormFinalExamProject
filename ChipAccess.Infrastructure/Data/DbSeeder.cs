using System;
using System.Linq;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (!db.Employees.Any())
            {
                db.Employees.AddRange(
                    new Employee
                    {
                        BamId = "aaaa0001",
                        FirstName = "Adam",
                        LastName = "Admin",
                        Email = "admin@test.local",
                        Role = "Admin",
                        IsActive = true
                    },
                    new Employee
                    {
                        BamId = "mmmm0001",
                        FirstName = "Mark",
                        LastName = "Manager",
                        Email = "manager@test.local",
                        Role = "Manager",
                        IsActive = true
                    },
                    new Employee
                    {
                        BamId = "uuuu0001",
                        FirstName = "User1",
                        LastName = "User",
                        Email = "user@test.local",
                        Role = "User",
                        IsActive = true
                    },
                    new Employee
                    {
                        BamId = "uuuu0001",
                        FirstName = "User2",
                        LastName = "User",
                        Email = "user@test.local",
                        Role = "User",
                        IsActive = true
                    }
                );

                db.SaveChanges();
            }

            if (db.AccessApprovalForms.Any())
                return;

            var rnd = new Random();

            for (int i = 1; i <= 10; i++)
            {
                var status = i switch
                {
                    <= 3 => AccessStatus.PendingManager,
                    <= 6 => AccessStatus.PendingAdmin,
                    <= 8 => AccessStatus.Active,
                    _ => AccessStatus.ExpiringSoon
                };

                db.AccessApprovalForms.Add(new AccessApprovalForm
                {
                    BamId = "uuuu0001",
                    Approver = "mmmm0001",
                    AccessNeeded = $"Test Access #{i}",
                    Reason = "Seeded test data",
                    CreatedDate = DateTime.UtcNow.AddDays(-rnd.Next(1, 20)),
                    ExpirationDate = DateTime.UtcNow.AddDays(rnd.Next(10, 90)),
                    Status = status,
                    ModifiedDate = DateTime.UtcNow
                });
            }

            db.SaveChanges();
        }
    }
}
