namespace Catalog.BLL.Exceptions
{
    public class BusinessConflictException : Exception
    {
        public BusinessConflictException(string message) : base(message) { }
    }
}

