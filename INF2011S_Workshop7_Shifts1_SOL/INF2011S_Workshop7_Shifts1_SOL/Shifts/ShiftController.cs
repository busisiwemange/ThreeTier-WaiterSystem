using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INF2011S_Workshop7_Shifts1_SOL.Employees;

namespace INF2011S_Workshop7_Shifts1_SOL.Shifts
{
    public class ShiftController
    {
        private List<Shift> newShift;
        private EmployeeController employeeController;

        public ShiftController(EmployeeController empController)
        {
            employeeController = empController;
            newShift = new List<Shift>();
        }

        public void NewShedule(System.DateTime StartDate, System.DateTime EndDate)
        {
            int count = 0;
            System.DateTime aDate = StartDate;

           
            for (count = 0; count <= 13; count++)
            {
                newShift.Add(new Shift());
                newShift[count].Date = aDate;
                newShift[count].ShiftDayEve = (Shift.ShiftType)(count % 2);
                newShift[count].Number = count + 1; 
                if (count % 2 == 1)
                {
                    aDate = aDate.AddDays(1);
                }
            }
        }
        public bool AddEmployeeToShift(int index, Employee emp)
        {
            bool addSuccessful = false;
            addSuccessful = newShift[index].Add2Shift(emp);
            return addSuccessful;
        }
    }
}
