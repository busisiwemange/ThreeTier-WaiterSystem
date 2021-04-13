using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using INF2011S_Workshop7_Shifts1_SOL.Employees;
using INF2011S_Workshop7_Shifts1_SOL.Shifts;

namespace INF2011S_Workshop7_Shifts1_SOL.PresentationLayer
{
    public partial class ShiftsForm : Form
    {

        private BlockArray shiftBookings;
        private BlockArray shiftDates;
        private EmployeeController employeeController;
        private ShiftController shiftController;
        public bool shiftsFormClosed = false;
        private bool comboInitialised = false;
        private System.DateTime startDate;
        private System.DateTime endDate;

        public ShiftsForm(EmployeeController empController)
        {
            InitializeComponent();
            this.Load += ShiftsForm_Load;
            this.FormClosed += ShiftsForm_Closed;
            employeeController = empController;
            shiftController = new ShiftController(employeeController);

        }
        #region Form Events
        private void ShiftsForm_Load(object sender, EventArgs e)
        {
            ShowControls(false);
            dayShiftsLabel.Text = Shift.ShiftType.Day.ToString();
            eveningShiftsLabel.Text = Shift.ShiftType.Evening.ToString();
            dayKeyLabel.BackColor = Color.LightGoldenrodYellow;
            eveningKeyLabel.BackColor = Color.Turquoise;
            calendarMessageLabel.BorderStyle = BorderStyle.None;
            calendarMessageLabel.ForeColor = Color.Red;
            calendarMessageLabel.Text = "Select a week to schedule shifts";
            shiftsMonthCalendar.DateSelected += ShiftsMonthCalendar_DateSelected;
        }

        private void ShiftsForm_Closed(object sender, FormClosedEventArgs e)
        {
            shiftsFormClosed = true;
        }
        #endregion

        private void ShiftsMonthCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            System.DateTime aDate = default(System.DateTime);
            //Calendar control functions to set the Start and End date
            startDate = shiftsMonthCalendar.SelectionRange.Start;
            endDate = shiftsMonthCalendar.SelectionRange.End;

            //***Add control array, ShiftDates, to show the dates of the shifts
            shiftDates = new BlockArray(this, 50, 100, 200, 1);
            aDate = startDate;

            int intcnt = 0;
            for (intcnt = 0; intcnt <= 6; intcnt++)
            {
                shiftDates.AddNewBlock();
                shiftDates.Item(intcnt).BackColor = Color.White;
                shiftDates.Item(intcnt).Text = aDate.ToShortDateString();
                //Show date on Button
                aDate = aDate.AddDays(1);
                //**This function allows you to go to the NEXT day
            }

            //***Add a block for each slot on all the shifts – ShiftBookings control array
            shiftBookings = new BlockArray(this, 50, 100, 300, 6);
            for (intcnt = 0; intcnt <= 41; intcnt++)
            {
                shiftBookings.AddNewBlock();
                if (intcnt % 6 > 2)
                {
                    shiftBookings.Item(intcnt).BackColor = Color.Turquoise;
                }
                //*** ONLY 4 slots on a SUNDAY (2 slots per shift)
                if (intcnt == 2 || intcnt == 5)
                {
                    shiftBookings.Item(intcnt).BackColor = Color.DarkSlateGray;
                    // shiftBookings.Item(intcnt).Enabled = false;
                }
                else
                {
                    //***NB NB Add a click event dynamically to make button respond on the click of the user  NB NB 
                    shiftBookings.Item(intcnt).Click += SlotSelected;
                }
            }

            shiftsMonthCalendar.Visible = false;
            calendarMessageLabel.Visible = false;
            FillCombo();
            //workshop 7 SHIFT CONTROLLER METHOD WILL BE CALLED HERE TO SAVE NEW SHIFT TO MEMORY AND
            shiftController.NewShedule(startDate, endDate);
            //THEN WRITE TO DATABASE--LATER
            ShowControls(true);
        }

        #region Utility  Methods
        private void ShowControls(bool value)
        {
            waitersComboBox.Visible = value;
            waitersLabel.Visible = value;
            dayShiftsLabel.Visible = value;
            eveningShiftsLabel.Visible = value;
            dayKeyLabel.Visible = value;
            eveningKeyLabel.Visible = value;
        }

        public void FillCombo()
        {
            Collection<Employee> waiters = null;
            //Find a collection of all the employees that are waiters
            // *** add  a second  find by role method that only needs one parameter
            waiters = employeeController.FindByRole(Role.RoleType.Waiter);
            //Link the objects in the collection of waitrons to every item of the combo box
            foreach (Employee employee in waiters)
            {
                waitersComboBox.Items.Add(employee);
            }

            //Set the current display of the combobox to nothing
            waitersComboBox.SelectedIndex = -1;
            waitersComboBox.Text = "";
            comboInitialised = true;
        }
        #endregion

        private void SlotSelected(System.Object sender, System.EventArgs e)
        {
            Employee employee = default(Employee);
            Button button = default(Button);
            int whichShift;

               
            button = (Button)sender;
                     
            employee = (Employee)waitersComboBox.SelectedItem;
            //Cannot book a slot if NO employee(waitron) is selected
            if (employee == null)
            {
                MessageBox.Show("First select a Waiter for the shift");
            }
            else
            {
                whichShift = (Convert.ToInt32(button.Tag) / 6) * 2 + (Convert.ToInt32(button.Tag) % 6) / 3;
                if (shiftController.AddEmployeeToShift(whichShift, employee))
                {  
                    button.Text = employee.Name;
                    button.BackColor = Color.Red;
                    button.Click -= SlotSelected;  
               }
            }
        }
    }
}
