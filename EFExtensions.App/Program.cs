using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFExtensions.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DatabaseTestActions databaseTestActions = new DatabaseTestActions();

            //databaseTestActions.UpdateEmployee();
            databaseTestActions.UpdatePLDate();

            //databaseTestActions.UpdateEmployeeFromList();
        }
    }
}

