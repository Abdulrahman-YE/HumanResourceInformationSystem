using DomainLayer.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.AccountServices
{
    public interface IAccountServices : IAccountRepository
    {
        void ValidateModelDataAnnotations(IAccountModel model);
    }
}
