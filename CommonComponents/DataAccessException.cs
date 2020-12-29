using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents
{
    public class DataAccessException : Exception
    {
        public DataAccessException()
        {

        }
        private DataAccessStatus dataAccessStatus { get; set; }

        public DataAccessStatus DataAccessStatusInfo
        {
            get { return this.dataAccessStatus; }
            set { this.dataAccessStatus = value; }
        }

        public DataAccessException(DataAccessStatus dataAccessStatus)
        {
            this.dataAccessStatus = dataAccessStatus;
        }

        public DataAccessException(string message, Exception innerException, DataAccessStatus dataAccessStatus) : base(message, innerException)
        {
            
            this.dataAccessStatus = dataAccessStatus;
        }
    }
}
