using EFExtensions.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFExtensions.App
{
    public class DatabaseTestActions
    {
        public int UpdateEmployeeFromList()
        {
            List<Employees> e = new List<Employees>
            {
                new Employees { EmployeeID = 2, Contact = "22222222", FirstName = "Atharva-2", LastName = "Singh" },
                new Employees { EmployeeID = 3, Contact = "33333333", FirstName = "Atharva-3", LastName = "Singh" },
                new Employees { EmployeeID = 4, Contact = "99999991", FirstName = "Atharva-4", LastName = "Singh" }
            };

            using (var db = new EFExtensionTestDatabaseEntities())
            {
                db.InsertOrUpdate(e);
            }

            return 0;
        }

        public int UpdateEmployee()
        {
            List<Employees> e = new List<Employees>();

            using (var db = new EFExtensionTestDatabaseEntities())
            {
                db.InsertOrUpdate<Employees>(u => new Employees { EmployeeID = 2, LastName = "Kumar" });
                db.InsertOrUpdate<Employees>(u => new Employees { EmployeeID = 3, Contact = "89898899", });
            }

            return 0;
        }

        internal void UpdatePLDate()
        {
            using (var db = new EFExtensionTestDatabaseEntities())
            {
                //db.InsertOrUpdate<PLDates>(u => new PLDates{ AppName = "Trader Supervision", Username = "singp57", Rundate = new DateTime(2023, 06, 21) });
                db.UpdateFromQuery<PLDates>(w => w.AppName == "Trader Supervision", u => new PLDates { Rundate = new DateTime(2023, 06, 24), Username = "yanr2" });
                //db.PLDates.Where(w => w.AppName == "Trader Supervision").UpdateFromQuery(u => u.Rundate = new DateTime(2023, 06, 26));
                //db.SaveChanges();
            }
        }
    }
}
