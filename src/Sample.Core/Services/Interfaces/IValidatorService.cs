namespace Sample.Core.Services.Interfaces
{
    public interface IValidatorService
    {
        void Validate<T>(T request);
    }
}