namespace ServiceLayer.CommonServices
{
    public interface IModelDataAnnotationCheck
    {
        void ValidateModelDataAnnotations<TDomainModel>(TDomainModel domainModel);
    }
}