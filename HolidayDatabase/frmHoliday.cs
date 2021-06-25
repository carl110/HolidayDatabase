using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HolidayDatabase
{
    public partial class frmHoliday : Form
    {
        List<Holiday> listOfHolidays;
        int recordNumber = 0;
        private PrintDocument myPrintDocument = new PrintDocument();
        private Font printFont;
        private int intPageCount;
        public frmHoliday()
        {
            InitializeComponent();
            myPrintDocument.BeginPrint += myPrintDocument_BeginPrint;
            myPrintDocument.PrintPage += myPrintDocument_PrintPage;
            printFont = new Font("Ariel", 10.0f);
            Text = "Task A Carl Wainwright " + DateTime.Now.ToShortDateString();
            ShowRecord();
        }
        private void updateList()
        {
            listOfHolidays = DataAccess.getAllHolidays();
        }
        private void ShowRecord()
        { //Method to show data according to list number called 
            updateList();
            if (listOfHolidays.Count > 0)
            {
                txtHolidayNumber.Text = listOfHolidays[recordNumber].HolidayNo.ToString();
                txtDestination.Text = listOfHolidays[recordNumber].Destination;
                txtCost.Text = string.Format(new CultureInfo("en-GB"), "{0:C}", listOfHolidays[recordNumber].Cost);
                txtDepartureDate.Text = listOfHolidays[recordNumber].Departure.ToString("dd/MM/yyyy");
                txtNoofDays.Text = listOfHolidays[recordNumber].NoOfDays.ToString();
                chkAvailable.Checked = listOfHolidays[recordNumber].Available;
                txtRecordCount.Text = (recordNumber + 1) + " of " + listOfHolidays.Count;
            }
            else
            {   //If no holidays exist show empty fields
                TraverseControlsAndSetTextEmpty(this);
                chkAvailable.Checked = false;
                txtRecordCount.Text = "No Records";
            }
        }
        private void TraverseControlsAndSetTextEmpty(Control control)
        {   //Method to clear all text fields in form call using -- TraverseControlsAndSetTextEmpty(this);
            foreach (Control c in control.Controls)
            {
                TextBox box = c as TextBox;
                if (box != null)
                {
                    box.Text = string.Empty;
                }
                this.TraverseControlsAndSetTextEmpty(c);
            }
        }
        private bool invalidFields(Control control)
        {
            DateTime value;
            //Check if any text fields are empty
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    TextBox textBox = c as TextBox;
                    if (textBox.Text == string.Empty)
                    {
                        return true;
                    }
                }
            }
            //Check if date is valid date
            if (DateTime.TryParse(txtDepartureDate.Text, out value) == false)
            {
                return true;
            }
            else return false;
        }
        private void myPrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            float linesPerPage = 0;
            float yPos = 0;
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            string holidayRecord = null;
            int currentPage = intPageCount + 1;
            Font printFont = new Font("Courier New", 14, FontStyle.Bold);
            // Calculate the number of lines per page.
            linesPerPage = e.MarginBounds.Height / printFont.GetHeight(e.Graphics);
            StringFormat centeredText = new StringFormat();
            centeredText.Alignment = StringAlignment.Center;
            // Title & Page Number
            string title = "Holiday List";
            e.Graphics.DrawString(title, printFont, Brushes.Black,
            e.PageSettings.PaperSize.Width / 2, yPos, centeredText);
            e.Graphics.DrawString($"Page {currentPage.ToString().PadLeft(2, '1')}", printFont, Brushes.Black, e.MarginBounds.Right, yPos);
            yPos += Convert.ToInt32(printFont.GetHeight());
            linesPerPage -= 1;
            // Today's Date
            printFont = new Font("Courier New", 10, FontStyle.Bold);
            string date = $"Date {DateTime.Today:d}";
            e.Graphics.DrawString(date, printFont, Brushes.Black, e.PageSettings.PaperSize.Width / 2, yPos, centeredText);
            yPos += Convert.ToInt32(printFont.GetHeight()) * 2;
            linesPerPage -= 1;
            // Column Headings
            string headings = string.Format(
                "{0} {1} {2} {3} {4} {5}",
                "Holiday#".PadRight(10),
                "Destination".PadRight(15),
                "Cost".PadRight(10),
                "Departure Date".PadRight(15),
                "#Days".PadRight(10),
                "Available".PadRight(6)
            );
            e.Graphics.DrawString(headings, printFont, Brushes.Black, 10, yPos);
            yPos += Convert.ToInt32(printFont.GetHeight());
            linesPerPage -= 1;
            //for each record in list
            for (int i = 0; recordNumber < listOfHolidays.Count && i < linesPerPage; recordNumber++, i++)
            {
                yPos = topMargin + (recordNumber * printFont.GetHeight());
                //combine list deatils into 1 line
                holidayRecord = string.Format(
                       "{0} {1} {2} {3} {4} {5}",
                       listOfHolidays[recordNumber].HolidayNo.ToString().PadRight(10),
                       listOfHolidays[recordNumber].Destination.PadRight(15),
                       String.Format(new CultureInfo("en-GB"), "{0:C}", listOfHolidays[recordNumber].Cost, 2).PadRight(10),
                       listOfHolidays[recordNumber].Departure.ToString("dd/MM/yyyy").PadRight(15),
                       listOfHolidays[recordNumber].NoOfDays.ToString().PadRight(10),
                       (listOfHolidays[recordNumber].Available ? "Yes" : "No").PadRight(6)
                       );
                e.Graphics.DrawString(holidayRecord, printFont, Brushes.Black, 10, yPos, new StringFormat());
            }
            //If more lines exist, print another page.
            if (listOfHolidays.Count > recordNumber)
            {
                e.HasMorePages = true;
                currentPage++;
            }
            else
                e.HasMorePages = false;
        }
        private void myPrintDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            recordNumber = 0;
        }
        private bool holNoExists()
        {   //check if holiday number exists in list
            for (int i = 0; i < listOfHolidays.Count; i++)
            {
                if (listOfHolidays[i].HolidayNo.ToString().Equals(txtHolidayNumber.Text))
                {
                    return true;
                }
            }
            return false;
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            bool exists = holNoExists();
            if (exists == true)
            {   //allow update if reg exists
                if (invalidFields(this) == false)
                {    //update sql table and show confiration message
                    Holiday updateHol = listOfHolidays[recordNumber];
                    {
                        updateHol.HolidayNo = Convert.ToInt32(txtHolidayNumber.Text);
                        updateHol.Destination = txtDestination.Text;
                        updateHol.Cost = Convert.ToDouble(txtCost.Text.Substring(1));
                        updateHol.Departure = Convert.ToDateTime(txtDepartureDate.Text);
                        updateHol.NoOfDays = Convert.ToInt32(txtNoofDays.Text);
                        updateHol.Available = Convert.ToBoolean(chkAvailable.Checked);
                    }
                    DataAccess.UpdateHoliday(updateHol);
                    ShowRecord();
                    MessageBox.Show("Your update has been processed", "Holiday updated", MessageBoxButtons.OK);
                }
                else
                {   //advise invalid field data
                    MessageBox.Show("One of your fields are invalid", "Invalid Field(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {   //message to advise reg doesnt exist
                MessageBox.Show("This holiday number does not exist, you may Add this holiday as a new record", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool duplicate = holNoExists();
            if (duplicate == true)
            {   //if exists then advise they can update
                MessageBox.Show("This Holiday Number already exists on the database, you may update but not add as new", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {   //check txt fields
                if (invalidFields(this) == false)
                {   //Add holiday to sql table and update ist
                    DataAccess.AddHoliday(txtHolidayNumber.Text, txtDestination.Text, txtCost.Text.Substring(1), txtDepartureDate.Text, txtNoofDays.Text, chkAvailable.Checked);
                    recordNumber = listOfHolidays.Count - 1;
                    ShowRecord();
                    MessageBox.Show("Your holiday has been added to the database", "Holiday Added", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("One of your fields are invalid", "Invalid Field(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void onlyAllowNumber(object sender)
        {//if text is not only numbers
            if (!int.TryParse((sender as TextBox).Text, out int result))
            {   //remove all non numerical entries
                (sender as TextBox).Text = Regex.Replace((sender as TextBox).Text, "[^0-9]", "");
            }
        }
        private void setAsSterlingAmmount(object sender)
        {
            string currencyString = "";
            //Loop through each character in string
            for (int i = 0; i < (sender as TextBox).Text.Length; i++)
            {
                currencyString += (sender as TextBox).Text[i];
                //If first character accepted is a decimal add a 0 before it
                if (currencyString == ".")
                {
                    currencyString = "0.";
                }
                //Check string is double, romove character if not
                if (!double.TryParse(currencyString, out double d))
                {
                    currencyString = currencyString.Remove(currencyString.Length - 1);
                }
            }
            if (!string.IsNullOrEmpty(currencyString))
            {   //Output string as Currency in sterling
                (sender as TextBox).Text = String.Format(new CultureInfo("en-GB"), "{0:C}", Convert.ToDouble(currencyString), 2);
            }
            else (sender as TextBox).Clear();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataAccess.DeleteHoliday(Convert.ToInt32(txtHolidayNumber.Text));
            ShowRecord();
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                myPrintDocument.Print();
                intPageCount++;
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ShowRecord();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnFirst_Click(object sender, EventArgs e)
        {
            recordNumber = 0;
            ShowRecord();
        }
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (recordNumber == 0)
            {
                recordNumber = listOfHolidays.Count - 1;
            }
            else
            {
                recordNumber -= 1;
            }
            ShowRecord();
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (recordNumber == listOfHolidays.Count - 1)
            {
                recordNumber = 0;
            }
            else
            {
                recordNumber += 1;
            }
            ShowRecord();
        }
        private void btnLast_Click(object sender, EventArgs e)
        {
            recordNumber = listOfHolidays.Count - 1;
            ShowRecord();
        }
        private void txtHolidayNumber_TextChanged(object sender, EventArgs e)
        {   
            onlyAllowNumber(sender);
        }
        private void txtHolidayNumber_KeyPress(object sender, KeyPressEventArgs e)
        {     //Allow only numbers on keypress
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void txtHolidayNumber_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty((sender as TextBox).Text))
            {
                //If number not between 200 - 1000 then show error and clear
                if (!Enumerable.Range(200, 801).Contains(Convert.ToInt32((sender as TextBox).Text)))
                {
                    MessageBox.Show("1: Holiday number must be in range 200 to 1000.", "Range Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    (sender as TextBox).Clear();
                }
            }
        }
        private void txtCost_KeyPress(object sender, KeyPressEventArgs e)
        {   //Allow numbers with upto one decimal point and £
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '£') && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '£') && ((sender as TextBox).Text.IndexOf('£') > -1))
            {
                e.Handled = true;
            }
        }
        private void txtCost_Leave(object sender, EventArgs e)
        {   
            if ((sender as TextBox).TextLength > 0)
            {
                setAsSterlingAmmount(sender);
            }
        }
        private void txtDepartureDate_KeyPress(object sender, KeyPressEventArgs e)
        {   //only allow numbers and forward slashes on key press
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
            {
                e.Handled = true;
            }
        }
        private void txtDepartureDate_Leave(object sender, EventArgs e)
        {   //Check date is in correct format
            Regex reg = new Regex(@"^(\d{1,2})/(\d{1,2})/(\d{4})$");
            Match m = reg.Match((sender as TextBox).Text);
            if (m.Success)
            {
                int dd = int.Parse(m.Groups[1].Value);
                int mm = int.Parse(m.Groups[2].Value);
                int yyyy = int.Parse(m.Groups[3].Value);
                try
                {
                    (sender as TextBox).Text = System.Convert.ToDateTime((sender as TextBox).Text).ToString("dd/MM/yyyy");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Invalid date", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    (sender as TextBox).Clear();
                }
            }
            else
            {
                MessageBox.Show("Wrong date format. The date should be in the format of dd/MM/yyyy ie. 21/02/2019", "Invalid date", MessageBoxButtons.OK, MessageBoxIcon.Error);
                (sender as TextBox).Clear();
            }
        }
        private void txtNoofDays_TextChanged(object sender, EventArgs e)
        {
            onlyAllowNumber(sender);
        }
        private void txtNoofDays_KeyPress(object sender, KeyPressEventArgs e)
        {   //Only allow numbers to be input on keypress
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
