using CommonComponents;
using DomainLayer.Models.Account;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ServiceLayer.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLayer.Services.AccountServices;
using System.Data;

namespace InfrastructureLayer.DataAccess.Repositories.Account
{
    public class AccountRepository : IAccountRepository
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



        public AccountRepository()
        {

        }

        public AccountRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void Add(IAccountModel model)
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
                        customMessage: "Unable to add Account. Could not open a database connection", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                }

                string addQuery =
                    "INSERT INTO Accounts (Username, Password, EmployeeID, RoleID) " +
                    "VALUES (@User, @Password, @EmployeeID, @RoleID)";

                using (SqlCommand cmd = new SqlCommand(null, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.Add);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Account could not be added because the username already exists.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = addQuery;

                    SqlParameter username = new SqlParameter("@User", System.Data.SqlDbType.VarChar);
                    SqlParameter password = new SqlParameter("@Password", System.Data.SqlDbType.VarChar);
                    SqlParameter employee = new SqlParameter("@EmployeeID", System.Data.SqlDbType.Int);
                    SqlParameter role = new SqlParameter("@RoleID", System.Data.SqlDbType.Int);

                    username.Value = model.Username;
                    password.Value = model.Password;
                    employee.Value = Convert.ToInt32(model.EmployeeID);
                    role.Value = Convert.ToInt32(model.RoleID);

                    cmd.Parameters.Add(username);
                    cmd.Parameters.Add(password);
                    cmd.Parameters.Add(employee);
                    cmd.Parameters.Add(role);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to add the account.", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm the Account Model was Added to the database
                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesExistInDB, RequestType.ConfirmAdd);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to find the account in database after add operation completed.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw new DataAccessException(dataAccessStatus);

                    }

                    sqlConnection.Close();
                }

            }

        }

        public IEnumerable<IAccountModel> GetAll()
        {
            List<AccountModel> departmentModels = new List<AccountModel>();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string selectAllQuery = "Select * FROM Accounts";


                    using (SqlCommand cmd = new SqlCommand(selectAllQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AccountModel account = new AccountModel();
                                account.ID = Int32.Parse(reader["ID"].ToString());
                                account.Username = reader["Username"].ToString();
                                account.Password = reader["Password"].ToString();
                                account.EmployeeID = Int32.Parse(reader["EmployeeID"].ToString());
                                account.RoleID = Int32.Parse(reader["RoleID"].ToString());

                                departmentModels.Add(account);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to get Accounts list from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                return departmentModels;
            }
        }

        public AccountModel GetByID(int id)
        {
            AccountModel account = new AccountModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool matchingRecoredFound = false;
            string selectByIdQuery = "SELECT Username, Password, EmployeeID, RoleID " +
                "FROM Accounts WHERE ID = @ID";

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
                                account.ID = id;
                                account.Username = reader["Username"].ToString();
                                account.Password = reader["Password"].ToString();
                                account.EmployeeID = Convert.ToInt32(reader["EmployeeID"].ToString());
                                account.RoleID = Convert.ToInt32(reader["RoleID"].ToString());
                            }

                        }

                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get the account record from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if (!matchingRecoredFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get account record for with id {id}. Id {id} does not exist in the database.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }

                return account;
            }


        }



        public void Remove(IAccountModel model)
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
                        customMessage: "Unable to Delete The Account. Could not open database connection.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string deleteQuery = "DELETE FROM Accounts WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesExistInDB, RequestType.Delete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Account "+ model.Username +" could not be deleted because it could not be found in the database";
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
                        customMessage: "Unable to Delete The Account.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm that the department model has been deleted

                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.ConfirmDelete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to Delete The Account in Database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }
                }
                sqlConnection.Close();
            }

        }

        public void Update(IAccountModel model)
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
                       customMessage: "Unable to Update Acocunt "+ model.Username+". Could not open database connection.",
                       helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string updateDepQuery =
                    "UPDATE Accounts " +
                    "SET Username = @User, " +
                    "Password = @Password, " +
                    "EmployeeID = @EmployeeID, " +
                    "RoleID = @RoleID " +
                    "WHERE ID =  @ID";

                using (SqlCommand cmd = new SqlCommand(updateDepQuery, sqlConnection))
                {
                    try
                    {
                        RecordExistsCheck(cmd, model, TypeOfExistenceCheck.DoesExistInDB, RequestType.Update);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Account "+ model.Username+" could not be updated because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = updateDepQuery;
                    SqlParameter user = new SqlParameter("@User", SqlDbType.VarChar, 20);
                    user.Value = model.Username;
                    SqlParameter password = new SqlParameter("@Password", SqlDbType.VarChar, 50);
                    password.Value = model.Password;

                    cmd.Parameters.Add(user);
                    cmd.Parameters.Add(password);
                    cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@RoleID", model.RoleID).SqlDbType = SqlDbType.Int;
                    cmd.Parameters.AddWithValue("@ID", model.ID).SqlDbType = SqlDbType.Int;

                    cmd.Prepare();

                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Update The Account",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                    }
                }
                sqlConnection.Close();
            }
        }

        public AccountModel GetByUsername(string username)
        {
            AccountModel account = new AccountModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool matchingRecoredFound = false;
            string selectByIdQuery = "SELECT ID, Password, EmployeeID, RoleID " +
                "FROM Accounts WHERE Username = @User";

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    using (SqlCommand cmd = new SqlCommand(selectByIdQuery, sqlConnection))
                    {
                        cmd.CommandText = selectByIdQuery;
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@User", username));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            matchingRecoredFound = reader.HasRows;
                            while (reader.Read())
                            {
                                account.ID = Convert.ToInt32(reader["ID"].ToString());
                                account.Username = username;
                                account.Password = reader["Password"].ToString();
                                account.EmployeeID = Convert.ToInt32(reader["EmployeeID"].ToString());
                                account.RoleID = Convert.ToInt32(reader["RoleID"].ToString());
                            }

                        }

                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get the account record from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if (!matchingRecoredFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get account record for with username : {username}. {username} does not exist in the database.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }

                return account;
            }
        }

            private bool RecordExistsCheck(
            SqlCommand cmd,
            IAccountModel model,
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
                cmdCheck.CommandText = "SELECT count(*) FROM Accounts where Username = @User";
                cmdCheck.Parameters.AddWithValue("@User", model.Username);
            }
            else if ((requestType == RequestType.Update) || (requestType == RequestType.Delete) || (requestType == RequestType.ConfirmDelete))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM Accounts WHERE ID = @ID";
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
