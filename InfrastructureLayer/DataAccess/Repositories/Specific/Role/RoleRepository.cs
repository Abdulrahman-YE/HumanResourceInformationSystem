using CommonComponents;
using DomainLayer.Models.Role;
using ServiceLayer.Services.RoleServices;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.DataAccess.Repositories.Specific
{
    public class RoleRepository : IRoleRepository
    {
        private string connectionString = Properties.Settings.Default.connectionStr;

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



        public RoleRepository()
        {

        }

        public RoleRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void Add(IRoleModel roleModel)
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
                        customMessage: "Unable to add Role Model. Could not open a database connection", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                }

                string addQuery =
                    "INSERT INTO [Roles] ([Name]) " +
                    "VALUES (@Name)";

                using (SqlCommand cmd = new SqlCommand(null, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, roleModel, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.Add);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Role model could not be added because it is already in the database.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = addQuery;

                    SqlParameter roleName = new SqlParameter("@Name", System.Data.SqlDbType.VarChar, 20);
                    roleName.Value = roleModel.Name;
                    cmd.Parameters.Add(roleName);
                    

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to add Role Model.", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm the Department Model was Added to the database
                    try
                    {
                        RecordExistsCheck(cmd, roleModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.ConfirmAdd);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to find role model in database after add operation completed.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw new DataAccessException(dataAccessStatus);

                    }

                    sqlConnection.Close();
                }

            }
        }

        public IEnumerable<IRoleModel> GetAll()
        {
            List<RoleModel> roleModels = new List<RoleModel>();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string selectAllQuery = "Select * FROM [Roles]";


                    using (SqlCommand cmd = new SqlCommand(selectAllQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RoleModel roleModel = new RoleModel();
                                roleModel.ID = Int32.Parse(reader["ID"].ToString());
                                roleModel.Name = string.Copy(reader["Name"].ToString());

                                roleModels.Add(roleModel);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to get Role Model list from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                return roleModels;
            }
        }

        public RoleModel GetByID(int id)
        {
            RoleModel roleModel = new RoleModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool matchingRecoredFound = false;

            string selectByIdQuery = "SELECT [ID], [Name]  " +
                "FROM [Roles] WHERE [ID] = @ID";

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
                                roleModel.ID = id;
                                roleModel.Name = reader["Name"].ToString();
                            }

                        }

                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get Role Model record from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if (!matchingRecoredFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get Role Model record for role id : {id}. Id : {id} does not exist in the database.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }

                return roleModel;
            }


        }

        public void Remove(IRoleModel roleModel)
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
                        customMessage: "Unable to Delete Role Model. Could not open database connection.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string deleteQuery = "DELETE FROM [Roles] WHERE [ID] = @ID";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, roleModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.Delete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Role model could not be deleted because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = deleteQuery;

                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@ID", roleModel.ID);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Delete Roles Model.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm that the department model has been deleted

                    try
                    {
                        RecordExistsCheck(cmd, roleModel, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.ConfirmDelete);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to role Department model in database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }
                }
                sqlConnection.Close();
            }

        }

        public void Update(IRoleModel roleModel)
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
                       customMessage: "Unable to Update Role Model. Could not open database connection.",
                       helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string updateDepQuery =
                    "UPDATE [Roles] " +
                    "SET [Name] = @Name " +
                    "WHERE [ID] =  @ID ";

                using (SqlCommand cmd = new SqlCommand(updateDepQuery, sqlConnection))
                {
                    try
                    {
                        RecordExistsCheck(cmd, roleModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.Update);
                    }
                    catch (DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "role model could not be updated because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = updateDepQuery;

                    cmd.Parameters.Add("@Name", System.Data.SqlDbType.VarChar, 40).Value = roleModel.Name;
                    cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = Convert.ToInt32(roleModel.ID);

                    cmd.Prepare();


                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Update Role Model.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                    }
                }
                sqlConnection.Close();
            }
        }

        private bool RecordExistsCheck(SqlCommand cmd, IRoleModel roleModel, TypeOfExistenceCheck typeOfExistenceCheck,
            RequestType requestType)
        {


            Int32 count0fRecordsFound = 0;
            bool recordExistsCheckPassed = true;

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            SqlCommand cmdCheck = new SqlCommand(null, cmd.Connection);
            cmdCheck.Prepare();


            if ((requestType == RequestType.Add) || (requestType == RequestType.ConfirmAdd))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM [Roles] where [Name] = @Name";
                cmdCheck.Parameters.Add(new SqlParameter("@Name", System.Data.SqlDbType.VarChar, 20)).Value = roleModel.Name;
            }
            else if ((requestType == RequestType.Update) || (requestType == RequestType.Delete) || (requestType == RequestType.ConfirmDelete))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM [Roles] WHERE [ID] = @ID";
                cmdCheck.Parameters.Add(new SqlParameter("@ID", roleModel.ID));

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
