using DomainLayer.Models.Account;
using ServiceLayer.CommonServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.AccountServices
{
    public class AccountServices : IAccountServices
    {
        private IAccountRepository repository;
        private IModelDataAnnotationCheck dataAnnotationCheck;

        public AccountServices()
        {

        }

        public AccountServices(IAccountRepository repository, IModelDataAnnotationCheck dataAnnotationCheck)
        {
            this.repository = repository;
            this.dataAnnotationCheck = dataAnnotationCheck;
        }

        public void Add(IAccountModel model)
        {
            ValidateModelDataAnnotations(model);
            repository.Add(model);
        }

        public IEnumerable<IAccountModel> GetAll()
        {
            return repository.GetAll();
        }

        public AccountModel GetByID(int id)
        {
            return repository.GetByID(id);
        }

        public void Remove(IAccountModel model)
        {
            repository.Remove(model);
        }

        public void Update(IAccountModel model)
        {
            ValidateModelDataAnnotations(model);
            repository.Update(model);
        }

        public AccountModel GetByUsername(string username)
        {
            return repository.GetByUsername(username);
        }

        public void ValidateModelDataAnnotations(IAccountModel model)
        {
            this.dataAnnotationCheck.ValidateModelDataAnnotations(model);
        }
    }
}
