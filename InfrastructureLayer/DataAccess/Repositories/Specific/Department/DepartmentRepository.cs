using DomainLayer.Models.Department;
using CommonComponents;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace InfrastructureLayer.DataAccess.Repositories.Specific.Department
{
    public class DepartmentRepository : IDepartmentRepository
    {

        protected string connectionString;

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



        public DepartmentRepository()
        {
            
        }

        public DepartmentRepository(string connectionString)
        {
            this.connectionString = connectionString;   
        }
        public void Add(IDepartmentModel departmentModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                try
                {
                    sqlConnection.Open();

                }
                catch(SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to add Department Model. Could not open a database connection", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                }

                string addQuery =
                    "INSERT INTO [HumanResource].[Departments] (DepartmentName, PhoneNumber, ManagerID) " +
                    "VALUES (@DepartmentName, @PhoneNumber, @ManagerId)";

                using (SqlCommand cmd = new SqlCommand(null, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, departmentModel, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.Add);
                    }
                    catch(DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Department model could not be added because it is already in the database.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = addQuery;

                    SqlParameter depName = new SqlParameter("@DepartmentName", System.Data.SqlDbType.NVarChar, 40);
                    SqlParameter phone = new SqlParameter("@PhoneNumber", System.Data.SqlDbType.NVarChar, 20);
                    SqlParameter manID = new SqlParameter("@ManagerId", System.Data.SqlDbType.Int);

                    depName.Value = departmentModel.DepartmentName;
                    phone.Value = departmentModel.PhoneNumber;
                    manID.Value = Convert.ToInt32((departmentModel.ManagerID));

  
                    cmd.Parameters.Add(depName);
                    cmd.Parameters.Add(phone);
                    cmd.Parameters.Add(manID);


                   

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch(SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to add Department Model.", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm the Department Model was Added to the database
                    try
                    {
                        RecordExistsCheck(cmd, departmentModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.ConfirmAdd);
                    }
                    catch(DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to find deaptment model in database after add o[eration completed.";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw new DataAccessException(dataAccessStatus);

                    }

                    sqlConnection.Close();
                }

            }
           
        }

        public IEnumerable<IDepartmentModel> GetAll()
        {
            List<DepartmentModel> departmentModels = new List<DepartmentModel>();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string selectAllQuery = "Select * FROM [HumanResource].[Departments]";
                    

                    using(SqlCommand cmd = new SqlCommand(selectAllQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader() )
                        {
                            while (reader.Read())
                            {


                                DepartmentModel departmentModel = new DepartmentModel();
                                departmentModel.DepartmentId = Int32.Parse(reader["DepartmentID"].ToString());
                                departmentModel.DepartmentName = reader["DepartmentName"].ToString();
                                departmentModel.PhoneNumber = reader["PhoneNumber"].ToString();
                                departmentModel.ManagerID = Int32.Parse(reader["ManagerID"].ToString());

                                departmentModels.Add(departmentModel);
                            }
                        }                     
                    }
                    sqlConnection.Close();
                }
                catch(SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to get Department Model list from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                        stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }
                return departmentModels;
            }
        }

        public DepartmentModel GetByID(int departmentId)
        {
            DepartmentModel departmentModel = new DepartmentModel();
            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            bool matchingRecoredFound = false;
            string selectByIdQuery = "SELECT [DepartmentID], [DepartmentName], [PhoneNumber], [ManagerID]  " +
                "FROM [HumanResource].[Departments] WHERE [DepartmentID] = @DepartmentID";

            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    using (SqlCommand cmd = new SqlCommand(selectByIdQuery, sqlConnection))
                    {
                        cmd.CommandText = selectByIdQuery;
                        cmd.Prepare();
                        cmd.Parameters.Add(new SqlParameter("@DepartmentID", departmentId));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            matchingRecoredFound = reader.HasRows;
                            while (reader.Read())
                            {
                                departmentModel.DepartmentId = departmentId;
                                departmentModel.DepartmentName = reader["DepartmentName"].ToString();
                                departmentModel.PhoneNumber = reader["PhoneNumber"].ToString();
                                departmentModel.ManagerID = Int32.Parse(reader["ManagerID"].ToString());
                            }

                        }

                    }
                    sqlConnection.Close();
                }
                catch (SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to get Department Model record from database", helpLink: e.HelpLink, errorCode: e.ErrorCode,
                       stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                if(!matchingRecoredFound)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: "",
                      customMessage: $"Record not found!. Unable to get Department Model record for department id {departmentId}. Id {departmentId} does not exist in the database.", helpLink: "", errorCode: 0,
                      stackTrace: "");

                    throw new DataAccessException(dataAccessStatus);
                }

                return departmentModel;
            }


            }

        

        public void Remove(IDepartmentModel departmentModel)
        {
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                try
                {
                    sqlConnection.Open();
                }
                catch(SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Delete Department Model. Could not open database connection.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);
                    
                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string deleteQuery = "DELETE FROM [HumanResource].[Departments] WHERE [DepartmentID] = @DepartmentId";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, sqlConnection))
                {

                    try
                    {
                        RecordExistsCheck(cmd, departmentModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.Delete);
                    }
                    catch(DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Department model could not be deleted because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = deleteQuery;

                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentModel.DepartmentId);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch(SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Delete Department Model.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                    }

                    //Confirm that the department model has been deleted

                    try
                    {
                        RecordExistsCheck(cmd, departmentModel, TypeOfExistenceCheck.DoesNotExistInDB, RequestType.ConfirmDelete);
                    }
                    catch(DataAccessException e)
                    {
                        e.DataAccessStatusInfo.Status = "Error";
                        e.DataAccessStatusInfo.OperationSucceeded = false;
                        e.DataAccessStatusInfo.CustomMessage = "Failed to delete Department model in database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }
                }
                sqlConnection.Close();
            }
           
        }

        public void Update(IDepartmentModel departmentModel)
        {
            int result = -1;
            DataAccessStatus dataAccessStatus = new DataAccessStatus();

            using(SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                }
                catch(SqlException e)
                {
                    dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                       customMessage: "Unable to Update Department Model. Could not open database connection.",
                       helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                    throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);
                }

                string updateDepQuery =
                    "UPDATE [HumanResource].[Departments]" +
                    "SET [DepartmentName] = @DepartmentName," +
                    "[PhoneNumber] = @PhoneNumber ," +
                    "[ManagerID] = @ManagerId " +
                    "WHERE [DepartmentID] =  @DepartmentId ";

                using (SqlCommand cmd = new SqlCommand(updateDepQuery, sqlConnection))
                {
                    try
                    {
                        RecordExistsCheck(cmd, departmentModel, TypeOfExistenceCheck.DoesExistInDB, RequestType.Update);
                    }
                    catch(DataAccessException e)
                    {
                        e.DataAccessStatusInfo.CustomMessage = "Department model could not be updated because it could not be found in the database";
                        e.DataAccessStatusInfo.ExceptionMessage = string.Copy(e.Message);
                        e.DataAccessStatusInfo.StackTrace = string.Copy(e.StackTrace);

                        throw e;
                    }

                    cmd.CommandText = updateDepQuery;

                    cmd.Parameters.Add("@DepartmentName", System.Data.SqlDbType.NVarChar, 40).Value = departmentModel.DepartmentName;
                    cmd.Parameters.Add("@PhoneNumber", System.Data.SqlDbType.NVarChar, 20).Value = departmentModel.PhoneNumber;
                    cmd.Parameters.Add("@ManagerId", System.Data.SqlDbType.Int).Value = Convert.ToInt32(departmentModel.ManagerID);
                    cmd.Parameters.Add("@DepartmentId", System.Data.SqlDbType.Int).Value = Convert.ToInt32(departmentModel.DepartmentId);

                    cmd.Prepare();


                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch(SqlException e)
                    {
                        dataAccessStatus.setValues(status: "Error", operationSucceeded: false, exceptionMessage: e.Message,
                        customMessage: "Unable to Update Department Model.",
                        helpLink: e.HelpLink, errorCode: e.ErrorCode, stackTrace: e.StackTrace);

                        throw new DataAccessException(e.Message, e.InnerException, dataAccessStatus);

                    }
                }
                sqlConnection.Close();
            }
        }

        private bool RecordExistsCheck(SqlCommand cmd, IDepartmentModel departmentModel, TypeOfExistenceCheck typeOfExistenceCheck,
            RequestType requestType)
        {
            

            Int32 count0fRecordsFound = 0;
            bool recordExistsCheckPassed = true;

            DataAccessStatus dataAccessStatus = new DataAccessStatus();
            SqlCommand cmdCheck = new SqlCommand(null, cmd.Connection);
            cmdCheck.Prepare();
            

            if((requestType == RequestType.Add) || (requestType == RequestType.ConfirmAdd))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM [HumanResource].[Departments] where [DepartmentName] = @DepName";
                cmdCheck.Parameters.Add(new SqlParameter("@DepName", System.Data.SqlDbType.NVarChar,40)).Value = departmentModel.DepartmentName;
            }
            else if((requestType == RequestType.Update) || (requestType == RequestType.Delete) || (requestType == RequestType.ConfirmDelete))
            {
                cmdCheck.CommandText = "SELECT count(*) FROM [HumanResource].[Departments] WHERE [DepartmentID] = @DepartId";
                cmdCheck.Parameters.Add(new SqlParameter("@DepartId", departmentModel.DepartmentId));

            }

            try
            {
                count0fRecordsFound = Convert.ToInt32(cmdCheck.ExecuteScalar());
            }
            catch(SqlException e)
            {
                string msg = e.Message;
                throw e;
            }

            if((typeOfExistenceCheck == TypeOfExistenceCheck.DoesNotExistInDB) && (count0fRecordsFound > 0))
            {
                dataAccessStatus.Status = "Error";
                recordExistsCheckPassed = false;

                throw new DataAccessException(dataAccessStatus);
            }
            else if((typeOfExistenceCheck == TypeOfExistenceCheck.DoesExistInDB) && (count0fRecordsFound == 0))
            {
                dataAccessStatus.Status = "Error";
                recordExistsCheckPassed = false;

                throw new DataAccessException(dataAccessStatus);
            }

            return recordExistsCheckPassed;
        }
    }
}
