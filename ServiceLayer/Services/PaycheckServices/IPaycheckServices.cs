using DomainLayer.Models.Paycheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.PaycheckServices
{
    interface IPaycheckServices : IPaycheckRepository
    {
        void ValidateModelDataAnnotations(IPaycheckModel model);
    }
}
