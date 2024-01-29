﻿using EAS_Buissness;
using Employees_Attendence_System.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Employees_Attendence_System.Employees
{
    public partial class FrmAddUpdateEmployee : Form
    {
        private enum enMode { add=1, update}
        private enMode _Mode = enMode.add;
        private clsEmployee _Employee;
        private clsPerson _Person;
        private int _PerosnID = -1;
        private int _EmployeeID = -1;

        public FrmAddUpdateEmployee(int EmployeeID = -1)
        {
            _EmployeeID = EmployeeID; 
            InitializeComponent();
            _Mode = _EmployeeID == -1 ? enMode.add : enMode.update;
        }
        
        private void _AssignInformation(clsEmployee Employee)
        {
            
        }
      
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                return; 

            if(_Mode == enMode.add)
            {
                _Person = new clsPerson();

                _Person.FirstName = txtFirstName.Text; 
                _Person.LastName = txtLastName.Text;
                _Person.Email = txtEmail.Text;
                _Person.SecondName = txtSecondName.Text;
                _Person.ThirdName = txtThirdName.Text;
                _Person.PhoneNumber = txtPhone.Text;
                _Person.BirthDate = dtpBirthdate.Value;
                _Person.Nationality = cbCountries.Text.Trim();
                _Person.Address = txtAddress.Text;
                _Person.Gender = rdFemale.Checked ? "Female" : "Male";

                if (_Person.Save())
                {
                    _PerosnID = _Person.ID; 
                    _Employee = new clsEmployee();
                    _Employee.PersonID = _PerosnID;
                    _Employee.EmployeeDepartmentID = cbDepartmentsOptions.SelectedIndex + 1;
                    _Employee.WorkedFrom = dtpHireDate.Value;

                    if (_Employee.Save())
                    {
                        _EmployeeID = _Employee.ID;
                        if(MessageBox.Show("Are you sure you want to save information?",
                            "Message Box", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            MessageBox.Show($"Employee with ID {_Employee.ID} saved successfully :-)",
                            "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            _Mode = enMode.update;
                            lblHeader.Text = "Update Employee";
                            lblID.Text = _PerosnID.ToString();
                            lblEmploymentID.Text = _EmployeeID.ToString();

                            return; 
                        }
                    }
                }
                
                MessageBox.Show("Something went wrong during saving information :-( , Try again later!",
                "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateEmptyTextBox(object sender, CancelEventArgs e)
        {
            //set autovalidate property
            TextBox Temp = (TextBox)sender;
            if (string.IsNullOrEmpty(Temp.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(Temp, "This is required");
            }
            else
                errorProvider1.SetError(Temp, null);
        }


        private void LoadEmployeeInfo(clsEmployee employee)
        {
            if (employee != null)
            {
                lblID.Text = employee.PersonID.ToString(); 
                txtFirstName.Text = employee.PersonInfo.FirstName;
                txtSecondName.Text = employee.PersonInfo.SecondName;
                txtThirdName.Text = employee.PersonInfo.ThirdName;
                txtLastName.Text = employee.PersonInfo.LastName;

                txtEmail.Text = employee.PersonInfo.Email;
                txtPhone.Text = employee.PersonInfo.PhoneNumber;
                txtAddress.Text = employee.PersonInfo.Address; 


                if (employee.PersonInfo.Gender == "Male")
                    rdMale.Checked = true;
                else
                    rdFemale.Checked = true;

                dtpBirthdate.Text = employee.PersonInfo.BirthDate.ToShortDateString();


                cbDepartmentsOptions.SelectedItem = employee.DepartmentInfo.Name.ToString();
                cbCountries.SelectedItem = employee.PersonInfo.Nationality.ToString();

                lblEmploymentID.Text = employee.ID.ToString();
                dtpHireDate.Text = employee.WorkedFrom.ToString();

                if(employee.WorkedTo == null)
                    dtpLeaveDate.Enabled = false;
                else
                    dtpLeaveDate.Text = employee.WorkedTo.ToString();
            }
            else
                MessageBox.Show("Employee NOT Found", "Message Box", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error); 
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetDefaultValues()
        {
            if (_Mode == enMode.add)
            {
                rdMale.Checked = true;
                dtpLeaveDate.Enabled = false;
            }
               

            //set the max date to 18 years from now 
            dtpBirthdate.MaxDate = DateTime.Now.AddDays(-18);
            //should not allow adding age more than 100 years
            dtpBirthdate.MinDate = DateTime.Now.AddYears(-100);


            //load countries list
            DataTable countries = clsPerson.CountriesList();
            foreach (DataRow row in countries.Rows)
            {
                cbCountries.Items.Add(row["nicename"]);
            }

            //load departments list 
            DataTable departments = clsDepartment.DepartmentsList();
            foreach (DataRow row in departments.Rows)
            {
                cbDepartmentsOptions.Items.Add(row["Name"]);
            }

            cbCountries.SelectedIndex = cbCountries.FindString("Jordan");
            cbDepartmentsOptions.SelectedIndex = cbDepartmentsOptions.FindString("Adminstration");
        }
        private void FrmAddUpdateEmployee_Load(object sender, EventArgs e)
        {
            SetDefaultValues();

            if (_Mode == enMode.add)
            {
                lblHeader.Text = "Add Employee";
                txtFirstName.Focus();
            }
            else
            {
                lblHeader.Text = "Update Employee";
                _Employee = clsEmployee.Find(_EmployeeID);
                LoadEmployeeInfo(_Employee); 
            }
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            if (!clsGlobal.ValidateEmail(txtEmail.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, "NOT a valid E-mail!");
            }
            else
                errorProvider1.SetError(txtEmail, null);
        }
    }
}