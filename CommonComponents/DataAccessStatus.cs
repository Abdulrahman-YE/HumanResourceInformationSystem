using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents
{
    public class DataAccessStatus
    {
        public string Status { get; set; }
        public bool OperationSucceeded { get; set; }
        public string ExceptionMessage { get; set; }

        public string CustomMessage { get; set; }
        public string HelpLink { get; set; }
        public int ErrorCode { get; set; }
        public string StackTrace { get; set; }

        public void setValues(string status, bool operationSucceeded,
                                string exceptionMessage, string customMessage,
                                string helpLink, int errorCode, 
                                string stackTrace)
        {
            this.Status = status ?? string.Copy("");
            this.OperationSucceeded = operationSucceeded;
            this.ExceptionMessage = exceptionMessage ?? string.Copy("");
            this.CustomMessage = customMessage ?? string.Copy("");
            this.HelpLink = helpLink ?? string.Copy("");
            this.ErrorCode = errorCode;
            this.StackTrace = stackTrace ?? string.Copy("");
        }

        public string getFormattedValues()
        {
            return $"Status--> {Status}\nOpeartionSucceeded--> {OperationSucceeded}" +
                $"\nExceptionMessage--> {ExceptionMessage}\nCustomMessage--> {CustomMessage}\nHelpLink--> {HelpLink}" +
                $"\nErrorCode--> {ErrorCode}\nStackTrace--> {StackTrace}";
        }
    }
}
