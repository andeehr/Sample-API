namespace Sample.Common.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public DuplicateKeyException()
            : base("Duplicate key")
        {
        }

        public DuplicateKeyException(string message)
            : base(message) { }
    }
}