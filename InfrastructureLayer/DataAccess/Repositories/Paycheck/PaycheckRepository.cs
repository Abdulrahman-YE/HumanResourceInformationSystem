using CommonComponents;
using DomainLayer.Models.Paycheck;
using ServiceLayer.Services.PaycheckServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.DataAccess.Repositories.Paycheck
{
    public class PaycheckRepository : IPaycheckRepository
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



        public PaycheckRepository()
        {

        }

        public PaycheckRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void Add(IPaycheckModel model)
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
                        customMessage: "Unable to add paycheck. Could not open a database connection", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                }

                string addQuery =
                    "INSERT INTO Paychecks (Amount, EmployeeID, PayrollID) " +
                    "VALUES (@Amount, @EmployeeID, @PayrollID)";

                using (SqlCommand cmd = new SqlCommand(null, sqlConnection))
                {

                    cmd.CommandText = addQuery;

                    cmd.Parameters.AddWithValue("@Amount", model.Amount).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@PayrollID", model.PayrollID).SqlDbType = SqlDbType.Int;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to add the paycheck.", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm the Paycheck Model was Added to the database
                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesExistInDB, RequestType.ConfirmAdd);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to find the paycheck in database after add operation completed.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw new DataAccessException(dataAccessStatus);

                    }

                    sqlConnection.Close();
                }

            }

        }

        public IEnumerable<IPaycheckModel> GetAll()
        {
            List<PaycheckModel> paychecks = new List<PaycheckModel>();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string selectAllQuery = "Select * FROM Paychecks";


                    using (SqlCommand cmd = new SqlCommand(selectAllQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PaycheckModel paycheck = new PaycheckModel();
                                paycheck.ID = Int32.Parse(reader["ID"].ToString());
                                paycheck.Amount = Int32.Parse(reader["Amount"].ToString());
                                paycheck.EmployeeID = Int32.Parse(reader["EmployeeID"].ToString());
                                paycheck.PayrollID = Int32.Parse(reader["PayrollID"].ToString());
                                paycheck.ReceiptionDate = (DateTime)reader["ReceiptionDate"];

                                paychecks.Add(paycheck);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to get Paychecks list from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                return paychecks;
            }
        }

        public PaycheckModel GetByID(int id)
        {
            PaycheckModel paycheck = new PaycheckModel ();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool matchingRecoredFound = false;
            string selectByIdQuery = "SELECT Amount, EmployeeID, PayrollID, ReceiptionDate " +
                "FROM Paychecks WHERE ID = @ID";

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
                                paycheck.ID = id;
                                paycheck.Amount = Int32.Parse(reader["Amount"].ToString());
                                paycheck.EmployeeID = Int32.Parse(reader["EmployeeID"].ToString());
                                paycheck.PayrollID = Int32.Parse(reader["PayrollID"].ToString());
                                paycheck.ReceiptionDate = (DateTime)reader["ReceiptionDate"];
                            }

                        }

                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get the paycheck record from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if (!matchingRecoredFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get paycheck record with id {id}. Id {id} does not exist in the database.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }
                return paycheck;
            }
        }



        public void Remove(IPaycheckModel model)
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
                        customMessage: "Unable to Delete The Paycheck. Could not open database connection.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string deleteQuery = "DELETE FROM Paychecks WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesExistInDB, RequestType.Delete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Paycheck could not be deleted because it could not be found in the database";
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

                    //Confirm that the Paycheck model has been deleted

                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.ConfirmDelete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to Delete The Paycheck in Database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }
                }
                sqlConnection.Close();
            }

        }

        public void Update(IPaycheckModel model)
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
                       customMessage: "Unable to Update Paycheck. Could not open database connection.",
                       helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string updateDepQuery =
                    "UPDATE Paychecks " +
                    "SET Amount = @Amount, " +
                    "EmployeeID = @EmployeeID, " +
                    "PayrollID = @PayrollID " +
                    "WHERE ID =  @ID";

                using (SqlCommand cmd = new SqlCommand(updateDepQuery, sqlConnection))
                {
                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesExistInDB, RequestType.Update);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "paycheck could not be updated because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = updateDepQuery;

                    cmd.Parameters.AddWithValue("@Amount", model.Amount).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@PayrollID", model.PayrollID).SqlDbType = SqlDbType.Int;
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
                        customMessage: "Unable to Update The Paycheck",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                    }
                }
                sqlConnection.Close();
            }
        }

        public IEnumerable<IPaycheckModel> GetByMonth(DateTime date)
        {
            List<PaycheckModel> paychecks = new List<PaycheckModel>();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool recordsFound = false;
            string selectQuery = "Select * FROM Paychecks WHERE Month(ReceiptionDate) = @Month AND YEAR(ReceiptionDate) = @Year ";

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get the paychecks records from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                using (SqlCommand cmd = new SqlCommand(selectQuery, sqlConnection))
                {
                    cmd.CommandText = selectQuery;
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@Month", date.Month);
                    cmd.Parameters.AddWithValue("@Year", date.Year);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        recordsFound = reader.HasRows;
                        while (reader.Read())
                        {
                            PaycheckModel paycheck = new PaycheckModel();
                            paycheck.ID = Convert.ToInt32(reader["ID"].ToString());
                            paycheck.EmployeeID = Convert.ToInt32(reader["EmployeeID"].ToString()); ;
                            paycheck.PayrollID = Convert.ToInt32(reader["PayrollID"].ToString());
                            paycheck.Amount = Convert.ToInt32(reader["Amount"].ToString());
                            paycheck.ReceiptionDate = (DateTime)reader["ReceiptionDate"];

                            paychecks.Add(paycheck);
                        }
                    }
                }
                sqlConnection.Close();

            }
                if (!recordsFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get Paycheck records in Month {date.Month}.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }
                return paychecks;
        }

        public IEnumerable<IPaycheckModel> GetByEmployee(int employeeID)
        {
            List<PaycheckModel> paychecks = new List<PaycheckModel>();

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool matchingRecoredFound = false;
            string selectByEmpQuery = "SELECT ID, Amount, PayrollID, ReceiptionDate " +
                "FROM Paychecks WHERE EmployeeID = @EmployeeID";

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    using (SqlCommand cmd = new SqlCommand(selectByEmpQuery, sqlConnection))
                    {
                        cmd.CommandText = selectByEmpQuery;
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            matchingRecoredFound = reader.HasRows;
                            while (reader.Read())
                            {
                                PaycheckModel paycheck = new PaycheckModel();
                                paycheck.ID = Convert.ToInt32(reader["ID"].ToString());
                                paycheck.EmployeeID = employeeID;
                                paycheck.PayrollID = Convert.ToInt32(reader["PayrollID"].ToString());
                                paycheck.Amount = Convert.ToInt32(reader["Amount"].ToString());
                                paycheck.ReceiptionDate = (DateTime)reader["ReceiptionDate"];

                                paychecks.Add(paycheck);
                            }

                        }

                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get the paychecks records from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if (!matchingRecoredFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get Paycheck record for with employee id: {employeeID}.It does not exist in the database.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }

                return paychecks;
            }
        }

        private bool RecordExistsCheck(
        SqlCommand cmd,
        IPaycheckModel model,
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
                cmdCheck.CommandText = "SELECT count(*) FROM Paychecks where ID = @ID";
                cmdCheck.Parameters.AddWithValue("@ID", model.ID);
            }
            else if ((requestType == RequestType.Update) || (requestType == RequestType.Delete) || (requestType == RequestType.ConfirmDelete))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM Paychecks WHERE ID = @ID";
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
