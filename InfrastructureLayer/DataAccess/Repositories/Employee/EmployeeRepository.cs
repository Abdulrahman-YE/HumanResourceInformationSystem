using CommonComponents;
using DomainLayer.Models;
using ServiceLayer.Services.EmployeeServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.DataAccess.Repositories.Employee
{
    public class EmployeeRepository : IEmployeeRepository
    {

        protected string connectionString = Properties.Settings.Default.connectionStr;

        /// <summary>
        /// Used to help if certin record exists or not
        /// </summary>
        public enum TypeOfExistenceCheck
        {
            DoesExistInDB,
            DoesNotExistInDB
        }

        public enum RequestType
        {
            Add,
            Update,
            Read,
            Delete,
            ConfirmAdd,
            ConfirmDelete

        }



        public EmployeeRepository()
        {

        }

        public EmployeeRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void Add(IEmployeeModel employeeModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                try
                {
                    sqlConnection.Open();

                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to add Employee Model. Could not open a database connection", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                }

                string addQuery =
                    "INSERT INTO Employees (Fullname, PhoneNumber, Address, Gender, Status, Country, DOB, Email, PersonalPhoto, DepartmentID) " +
                    "VALUES (@Name, @PhoneNumber, @Address, @Gender, @Status, @Country, @DOB, @Email, @PersonalPhoto, @DepartmentID)";

                using (SqlCommand cmd = new SqlCommand(null, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, employeeModel, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.Add);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Employee model could not be added because an employee with the same email is already in the database.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = addQuery;

                    SqlParameter name = new SqlParameter("@Name", System.Data.SqlDbType.VarChar, 40);
                    SqlParameter phone = new SqlParameter("@PhoneNumber", System.Data.SqlDbType.VarChar, 20);
                    SqlParameter address = new SqlParameter("@Address", System.Data.SqlDbType.VarChar);
                    SqlParameter gender = new SqlParameter("@Gender", System.Data.SqlDbType.VarChar);
                    SqlParameter status = new SqlParameter("@Status", System.Data.SqlDbType.VarChar);
                    SqlParameter country = new SqlParameter("@Country", System.Data.SqlDbType.VarChar);
                    SqlParameter dob = new SqlParameter("@DOB", System.Data.SqlDbType.Date);
                    SqlParameter email = new SqlParameter("@Email", System.Data.SqlDbType.VarChar);
                    SqlParameter photo = new SqlParameter("@PersonalPhoto", System.Data.SqlDbType.VarBinary);
                    SqlParameter depId = new SqlParameter("@DepartmentID", System.Data.SqlDbType.Int);




                    name.Value = employeeModel.Fullname;
                    phone.Value = employeeModel.PhoneNumber;
                    address.Value = employeeModel.Address;
                    gender.Value = employeeModel.Gender;
                    country.Value = employeeModel.Country;
                    dob.Value = employeeModel.DOB;
                    email.Value = employeeModel.Email;


                    if (!string.IsNullOrEmpty(employeeModel.Status))
                    {
                        status.Value = employeeModel.Status;

                    }
                    else
                    {
                        status.Value = DBNull.Value;
                    }

                    if (employeeModel.PersonalPhoto.Length > 0)
                    {
                        photo.Value = employeeModel.PersonalPhoto;
                    }
                    else
                    {
                        photo.Value = DBNull.Value;

                    }

                    if (employeeModel.DepartmentID != 0)
                    {
                        depId.Value = employeeModel.DepartmentID;
                    }
                    else
                    {
                        depId.Value = DBNull.Value;

                    }
                    cmd.Parameters.Add(name);
                    cmd.Parameters.Add(phone);
                    cmd.Parameters.Add(address);
                    cmd.Parameters.Add(gender);
                    cmd.Parameters.Add(status);
                    cmd.Parameters.Add(country);
                    cmd.Parameters.Add(email);
                    cmd.Parameters.Add(dob);
                    cmd.Parameters.Add(photo);
                    cmd.Parameters.Add(depId);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to add Employee Model.", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm the Employee Model was Added to the database
                    try
                    {
                        RecordExistsCheck(cmd, employeeModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.ConfirmAdd);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to find employee model in database after add operation completed.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw new DataAccessException(dataAccessStatus);

                    }

                    sqlConnection.Close();
                }

            }

        }

        public IEnumerable<IEmployeeModel> GetAll()
        {
            List<EmployeeModel> employeeModels = new List<EmployeeModel>();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string selectAllQuery = "Select * FROM Employees";


                    using (SqlCommand cmd = new SqlCommand(selectAllQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {


                                EmployeeModel employee = new EmployeeModel();
                                employee.ID = Int32.Parse(reader["ID"].ToString());
                                employee.Fullname = reader["Fullname"].ToString();
                                employee.PhoneNumber = reader["PhoneNumber"].ToString();
                                employee.Address = reader["Address"].ToString();
                                employee.Gender = reader["Gender"].ToString();
                                employee.Email = reader["Email"].ToString();
                                employee.DOB = (DateTime)reader["DOB"];
                                employee.Country = reader["Country"].ToString();
                                string status = reader["Status"].ToString();
                                Byte[] photo = (Byte[])reader["PersonalPhoto"];
                                string depID = reader["DepartmentID"].ToString();


                                if (!string.IsNullOrEmpty(depID))
                                {
                                    employee.DepartmentID = Int32.Parse(depID);

                                }
                                if (!string.IsNullOrEmpty(status))
                                {
                                    employee.Status = status;
                                }
                                if (photo.Length > 0)
                                {
                                    employee.PersonalPhoto = photo;
                                }

                                employeeModels.Add(employee);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to get Employee Model list from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                return employeeModels;
            }
        }

        public EmployeeModel GetByID(int employeeID)
        {
            EmployeeModel employee = new EmployeeModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool matchingRecoredFound = false;
            string selectByIdQuery = "SELECT *  " +
                "FROM Employees WHERE ID = @ID";

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    using (SqlCommand cmd = new SqlCommand(selectByIdQuery, sqlConnection))
                    {
                        cmd.CommandText = selectByIdQuery;
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@ID", employeeID));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            matchingRecoredFound = reader.HasRows;
                            while (reader.Read())
                            {
                                employee.ID = Int32.Parse(reader["ID"].ToString());
                                employee.Fullname = reader["Fullname"].ToString();
                                employee.PhoneNumber = reader["PhoneNumber"].ToString();
                                employee.Address = reader["Address"].ToString();
                                employee.Gender = reader["Gender"].ToString();
                                employee.Email = reader["Email"].ToString();
                                employee.DOB = (DateTime)reader["DOB"];
                                employee.Country = reader["Country"].ToString();
                                string status = reader["Status"].ToString();
                                Byte[] photo = (Byte[])reader["PersonalPhoto"];
                                string depID = reader["DepartmentID"].ToString();


                                if (!string.IsNullOrEmpty(depID))
                                {
                                    employee.DepartmentID = Int32.Parse(depID);

                                }
                                if (!string.IsNullOrEmpty(status))
                                {
                                    employee.Status = status;
                                }
                                if (photo.Length > 0)
                                {
                                    employee.PersonalPhoto = photo;
                                }
                            }

                        }

                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get Employee Model record from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if (!matchingRecoredFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get Emplyee Model record for employee id {employeeID}. Id {employeeID} does not exist in the database.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }

                return employee;
            }


        }



        public void Remove(IEmployeeModel employeeModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                try
                {
                    sqlConnection.Open();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Delete Employee Model. Could not open database connection.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string deleteQuery = "DELETE FROM Employees WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, employeeModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.Delete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Employee model could not be deleted because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = deleteQuery;

                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@ID", employeeModel.ID);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Delete Employee Model.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm that the employee model has been deleted

                    try
                    {
                        RecordExistsCheck(cmd, employeeModel, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.ConfirmDelete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to delete employee model in database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }
                }
                sqlConnection.Close();
            }

        }

        public void Update(IEmployeeModel employeeModel)
        {
            int result = -1;
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to Update Employee Model. Could not open database connection.",
                       helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }


                string updateQuery =
                    "UPDATE Employees " +
                    "SET Fullname = @Name , " +
                    "PhoneNumber = @PhoneNumber ," +
                    "DepartmentID = @DepartmentID ," +
                    "Address = @Address , " +
                    "Gender = @Gender , " +
                    "Status = @Status , " +
                    "Email = @Email , " +
                    "PersonalPhoto = @Photo , " +
                    "Country = @Country , " +
                    "DOB = @DOB " +
                    "WHERE ID =  @ID ";

                using (SqlCommand cmd = new SqlCommand(updateQuery, sqlConnection))
                {
                    try
                    {
                        RecordExistsCheck(cmd, employeeModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.Update);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Employee model could not be updated because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    try
                    {

                        if(!checkEmail(cmd, employeeModel))
                        {
                            cmd.CommandText = updateQuery;

                            SqlParameter name = new SqlParameter("@Name", System.Data.SqlDbType.VarChar, 40);
                            SqlParameter phone = new SqlParameter("@PhoneNumber", System.Data.SqlDbType.VarChar, 20);
                            SqlParameter address = new SqlParameter("@Address", System.Data.SqlDbType.VarChar, 50);
                            SqlParameter gender = new SqlParameter("@Gender", System.Data.SqlDbType.VarChar, 7);
                            SqlParameter status = new SqlParameter("@Status", System.Data.SqlDbType.VarChar, 15);
                            SqlParameter country = new SqlParameter("@Country", System.Data.SqlDbType.VarChar, 20);
                            SqlParameter dob = new SqlParameter("@DOB", System.Data.SqlDbType.Date);
                            SqlParameter email = new SqlParameter("@Email", System.Data.SqlDbType.VarChar, 50);
                            SqlParameter photo = new SqlParameter("@Photo", System.Data.SqlDbType.VarBinary, -1);
                            SqlParameter depId = new SqlParameter("@DepartmentID", System.Data.SqlDbType.Int);
                            SqlParameter id = new SqlParameter("@ID", System.Data.SqlDbType.Int);




                            name.Value = employeeModel.Fullname;
                            phone.Value = employeeModel.PhoneNumber;
                            address.Value = employeeModel.Address;
                            gender.Value = employeeModel.Gender;
                            country.Value = employeeModel.Country;
                            dob.Value = employeeModel.DOB;
                            email.Value = employeeModel.Email;
                            id.Value = employeeModel.ID;


                            if (!string.IsNullOrEmpty(employeeModel.Status))
                            {
                                status.Value = employeeModel.Status;

                            }
                            else
                            {
                                status.Value = DBNull.Value;
                            }

                            if (employeeModel.PersonalPhoto.Length > 0)
                            {
                                photo.Value = employeeModel.PersonalPhoto;
                            }
                            else
                            {
                                photo.Value = DBNull.Value;

                            }

                            if (employeeModel.DepartmentID != 0)
                            {
                                depId.Value = employeeModel.DepartmentID;
                            }
                            else
                            {
                                depId.Value = DBNull.Value;

                            }
                            cmd.Parameters.Add(name);
                            cmd.Parameters.Add(phone);
                            cmd.Parameters.Add(address);
                            cmd.Parameters.Add(gender);
                            cmd.Parameters.Add(status);
                            cmd.Parameters.Add(country);
                            cmd.Parameters.Add(dob);
                            cmd.Parameters.Add(photo);
                            cmd.Parameters.Add(depId);
                            cmd.Parameters.Add(id);
                        }
                        else
                        {
                            updateQuery =
                                "UPDATE Employees " +
                                "SET Fullname = @Name , " +
                                "PhoneNumber = @PhoneNumber ," +
                                "DepartmentID = @DepartmentID ," +
                                "Address = @Address , " +
                                "Gender = @Gender , " +
                                "Status = @Status , " +
                                "PersonalPhoto = @Photo , " +
                                "Country = @Country , " +
                                "DOB = @DOB " +
                                "WHERE ID =  @ID ";

                            cmd.CommandText = updateQuery;

                            SqlParameter name = new SqlParameter("@Name", System.Data.SqlDbType.VarChar, 40);
                            SqlParameter phone = new SqlParameter("@PhoneNumber", System.Data.SqlDbType.VarChar, 20);
                            SqlParameter address = new SqlParameter("@Address", System.Data.SqlDbType.VarChar, 50);
                            SqlParameter gender = new SqlParameter("@Gender", System.Data.SqlDbType.VarChar, 7);
                            SqlParameter status = new SqlParameter("@Status", System.Data.SqlDbType.VarChar, 15);
                            SqlParameter country = new SqlParameter("@Country", System.Data.SqlDbType.VarChar, 20);
                            SqlParameter dob = new SqlParameter("@DOB", System.Data.SqlDbType.Date);
                            SqlParameter photo = new SqlParameter("@Photo", System.Data.SqlDbType.VarBinary, -1);
                            SqlParameter depId = new SqlParameter("@DepartmentID", System.Data.SqlDbType.Int);
                            SqlParameter id = new SqlParameter("@ID", System.Data.SqlDbType.Int);




                            name.Value = employeeModel.Fullname;
                            phone.Value = employeeModel.PhoneNumber;
                            address.Value = employeeModel.Address;
                            gender.Value = employeeModel.Gender;
                            country.Value = employeeModel.Country;
                            dob.Value = employeeModel.DOB;
                            id.Value = employeeModel.ID;


                            if (!string.IsNullOrEmpty(employeeModel.Status))
                            {
                                status.Value = employeeModel.Status;

                            }
                            else
                            {
                                status.Value = DBNull.Value;
                            }

                            if (employeeModel.PersonalPhoto.Length > 0)
                            {
                                photo.Value = employeeModel.PersonalPhoto;
                            }
                            else
                            {
                                photo.Value = DBNull.Value;

                            }

                            if (employeeModel.DepartmentID != 0)
                            {
                                depId.Value = employeeModel.DepartmentID;
                            }
                            else
                            {
                                depId.Value = DBNull.Value;

                            }
                            cmd.Parameters.Add(name);
                            cmd.Parameters.Add(phone);
                            cmd.Parameters.Add(address);
                            cmd.Parameters.Add(gender);
                            cmd.Parameters.Add(status);
                            cmd.Parameters.Add(country);
                            cmd.Parameters.Add(dob);
                            cmd.Parameters.Add(photo);
                            cmd.Parameters.Add(depId);
                            cmd.Parameters.Add(id);
                        }
                    }
                    catch(DataAccessException e)
                    {
                        throw e;
                    }

                    

                    cmd.Prepare();
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Update Employee Model.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                    }
                }
                sqlConnection.Close();
            }
        }

        private bool RecordExistsCheck(SqlCommand cmd, IEmployeeModel employeeModel, TypeOfExistenceCheck typeOfExistenceCheck,
            RequestType requestType)
        {


            Int32 count0fRecordsFound = 0;
            bool recordExistsCheckPassed = true;

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            SqlCommand cmdCheck = new SqlCommand(null, cmd.Connection);
            cmdCheck.Prepare();


            if ((requestType == RequestType.Add) || (requestType == RequestType.ConfirmAdd))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM Employees where Email = @Email";
                cmdCheck.Parameters.Add(new SqlParameter("@Email", System.Data.SqlDbType.NVarChar, 40)).Value = employeeModel.Email;
            }
            else if ((requestType == RequestType.Update) || (requestType == RequestType.Delete) || (requestType == RequestType.ConfirmDelete))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM Employees WHERE ID = @ID";
                cmdCheck.Parameters.Add(new SqlParameter("@ID", employeeModel.ID));

            }

            try
            {
                count0fRecordsFound = Convert.ToInt32(cmdCheck.ExecuteScalar());
            }
            catch (SqlException e)
            {
                string msg = e.Message;
                throw e;
            }

            if ((typeOfExistenceCheck == TypeOfExistenceCheck.DoesNotExistInDB) && (count0fRecordsFound > 0))
            {
                dataAccessStatus.Status = "Error";
                recordExistsCheckPassed = false;

                throw new DataAccessException(dataAccessStatus);
            }
            else if ((typeOfExistenceCheck == TypeOfExistenceCheck.DoesExistInDB) && (count0fRecordsFound == 0))
            {
                dataAccessStatus.Status = "Error";
                recordExistsCheckPassed = false;

                throw new DataAccessException(dataAccessStatus);
            }

            return recordExistsCheckPassed;
        }


        private bool checkEmail(SqlCommand cmd, IEmployeeModel employeeModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool checkPass = false;

            SqlCommand cmdCheck = new SqlCommand(null, cmd.Connection);

            string query = "SELECT ID, email FROM Employees WHERE ID = @ID OR Email = @Email ";

            cmdCheck.CommandText = query;
            cmdCheck.Prepare();
            cmdCheck.Parameters.AddWithValue("@ID", employeeModel.ID);
            cmdCheck.Parameters.AddWithValue("@Email", employeeModel.Email);

            try
            {
                using (SqlDataReader reader = cmdCheck.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Int32.Parse(reader["ID"].ToString());
                        string email = reader["Email"].ToString();

                        if (id == employeeModel.ID)
                        {
                            if (email.Equals(employeeModel.Email))
                            {
                                checkPass = true;
                            }
                            else
                            {
                                checkPass = false;
                            }
                        }
                        else
                        {
                            dataAccessStatus.Status = "Error";
                            dataAccessStatus.CustomMessage = "Unable to update employee because another employee with the same email exists.";
                            dataAccessStatus.OperationSucceeded = false;

                            throw new DataAccessException(dataAccessStatus);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                string msg = e.Message;
                throw e;
            }

            return checkPass;
        }


    }
}