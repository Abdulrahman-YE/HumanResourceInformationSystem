using CommonComponents;
using DomainLayer.Models.Payroll;
using ServiceLayer.Services.PyrollServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.DataAccess.Repositories.Payroll
{
    public class PayrollRepository : IPayrollRepository
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



        public PayrollRepository()
        {

        }

        public PayrollRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void Add(IPayrollModel model)
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
                        customMessage: "Unable to add Payroll. Could not open a database connection", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                }

                string addQuery =
                    "INSERT INTO Payrolls (GrossPay, NetPay, EmployeeID) " +
                    "VALUES (@GrossPay, @NetPay, @EmployeeID)";

                using (SqlCommand cmd = new SqlCommand(null, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.Add);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Payroll could not be added because the employee has already payroll.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = addQuery;

                    cmd.Parameters.AddWithValue("@GrossPay", model.GrossPay).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@NetPay", model.NetPay).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID).SqlDbType = SqlDbType.Int;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to add the payroll.", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm the Payroll Model was Added to the database
                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesExistInDB, RequestType.ConfirmAdd);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to find the payroll in database after add operation completed.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw new DataAccessException(dataAccessStatus);

                    }

                    sqlConnection.Close();
                }

            }

        }

        public IEnumerable<IPayrollModel> GetAll()
        {
            List<PayrollModel> payrolls = new List<PayrollModel>();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string selectAllQuery = "Select * FROM Payrolls";


                    using (SqlCommand cmd = new SqlCommand(selectAllQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PayrollModel payroll = new PayrollModel();
                                payroll.ID = Int32.Parse(reader["ID"].ToString());
                                payroll.GrossPay = Int32.Parse(reader["GrossPay"].ToString());
                                payroll.NetPay = Int32.Parse(reader["NetPay"].ToString());
                                payroll.EmployeeID = Int32.Parse(reader["EmployeeID"].ToString());

                                payrolls.Add(payroll);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to get Payrolls list from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                return payrolls;
            }
        }

        public PayrollModel GetByID(int id)
        {
            PayrollModel payroll = new PayrollModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool matchingRecoredFound = false;
            string selectByIdQuery = "SELECT GrossPay, NetPay, EmployeeID " +
                "FROM Payrolls WHERE ID = @ID";

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    using (SqlCommand cmd = new SqlCommand(selectByIdQuery, sqlConnection))
                    {
                        cmd.CommandText = selectByIdQuery;
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@ID", id));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            matchingRecoredFound = reader.HasRows;
                            while (reader.Read())
                            {
                                payroll.ID = id;
                                payroll.GrossPay = Int32.Parse(reader["GrossPay"].ToString());
                                payroll.NetPay = Int32.Parse(reader["NetPay"].ToString());
                                payroll.EmployeeID = Int32.Parse(reader["EmployeeID"].ToString());
                            }

                        }

                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get the payroll record from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if (!matchingRecoredFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get payroll record with id {id}. Id {id} does not exist in the database.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }
                return payroll;
            }
        }



        public void Remove(IPayrollModel model)
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
                        customMessage: "Unable to Delete The Payroll. Could not open database connection.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string deleteQuery = "DELETE FROM Payrolls WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesExistInDB, RequestType.Delete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Payroll could not be deleted because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = deleteQuery;

                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@ID", model.ID);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Delete The Payroll.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm that the Payroll model has been deleted

                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.ConfirmDelete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to Delete The Payroll in Database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }
                }
                sqlConnection.Close();
            }

        }

        public void Update(IPayrollModel model)
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
                       customMessage: "Unable to Update Payroll. Could not open database connection.",
                       helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string updateDepQuery =
                    "UPDATE Payrolls " +
                    "SET GrossPay = @GrossPay, " +
                    "NetPay = @NetPay, " +
                    "EmployeeID = @EmployeeID " +
                    "WHERE ID =  @ID";

                using (SqlCommand cmd = new SqlCommand(updateDepQuery, sqlConnection))
                {
                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesExistInDB, RequestType.Update);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Payroll could not be updated because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = updateDepQuery;

                    cmd.Parameters.AddWithValue("@GrossPay", model.GrossPay).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@NetPay", model.NetPay).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@ID", model.ID).SqlDbType = SqlDbType.Int;

                    cmd.Prepare();
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Update The Payroll",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                    }
                }
                sqlConnection.Close();
            }
        }

        public PayrollModel GetByEmployee(int employeeID)
        {
            PayrollModel account = new PayrollModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool matchingRecoredFound = false;
            string selectByIdQuery = "SELECT ID, GrossPay, NetPay " +
                "FROM Payrolls WHERE EmployeeID = @EmployeeID";

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    using (SqlCommand cmd = new SqlCommand(selectByIdQuery, sqlConnection))
                    {
                        cmd.CommandText = selectByIdQuery;
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            matchingRecoredFound = reader.HasRows;
                            while (reader.Read())
                            {
                                account.ID = Convert.ToInt32(reader["ID"].ToString());
                                account.EmployeeID = employeeID;
                                account.GrossPay = Convert.ToInt32(reader["GrossPay"].ToString());
                                account.NetPay = Convert.ToInt32(reader["NetPay"].ToString());
                            }

                        }

                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get the Payroll record from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if (!matchingRecoredFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get Payroll record for with employee id: {employeeID}.It does not exist in the database.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }

                return account;
            }
        }

        private bool RecordExistsCheck(
        SqlCommand cmd,
        IPayrollModel model,
        TypeOfExistenceCheck typeOfExistenceCheck,
        RequestType requestType
        )
        {
            Int32 count0fRecordsFound = 0;
            bool recordExistsCheckPassed = true;

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            SqlCommand cmdCheck = new SqlCommand(null, cmd.Connection);
            cmdCheck.Prepare();

            if ((requestType == RequestType.Add) || (requestType == RequestType.ConfirmAdd))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM Payrolls where EmployeeID = @EmployeeID";
                cmdCheck.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
            }
            else if ((requestType == RequestType.Update) || (requestType == RequestType.Delete) || (requestType == RequestType.ConfirmDelete))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM Payrolls WHERE ID = @ID";
                cmdCheck.Parameters.AddWithValue("@ID", model.ID);

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
    }
}
