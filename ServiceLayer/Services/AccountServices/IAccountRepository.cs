using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Models.Account;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.AccountServices
{
    public interface IAccountRepository
    {
        void Add(IAccountModel model);
        void Update(IAccountModel model);
        void Remove(IAccountModel model);
        IEnumerable<IAccountModel> GetAll();
        AccountModel GetByID(int id);
        AccountModel GetByUsername(String username);
    }
}
