using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServiceLayer.CommonServices
{
    public class ModelDataAnnotationCheck : IModelDataAnnotationCheck
    {
        public void ValidateModelDataAnnotations<TDomainModel>(TDomainModel domainModel)
        {
            //Used to hold the validation results
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            //Define what will be validated
            ValidationContext validationContext = new ValidationContext(domainModel);

            //Store our validation results
            StringBuilder stringBuilder = new StringBuilder();

            if (!Validator.TryValidateObject(domainModel, validationContext, validationResults, validateAllProperties: true))
            {
                foreach (ValidationResult validationResult in validationResults)
                {
                    stringBuilder.Append(validationResult.ErrorMessage)
                        .AppendLine();
                }
            }

            //If there is an error
            if (validationResults.Count > 0)
            {
                throw new ArgumentException(stringBuilder.ToString());
            }
        }
    }
}
