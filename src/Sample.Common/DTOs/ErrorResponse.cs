namespace Sample.Common.DTOs
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public IEnumerable<ValidationError>? ValidationErrors { get; set; }
    }

    public class ValidationError
    {
        public string FieldName { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}