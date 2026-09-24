namespace Sample.Common.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException()
            : base("Forbidden access to the specified resource")
        {
        }

        public ForbiddenException(string message)
            : base(message) { }
    }
}