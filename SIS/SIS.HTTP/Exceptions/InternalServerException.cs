namespace SIS.HTTP.Exceptions
{
    using System;

    public class InternalServerException : Exception
    {
        public const string InternalServerExceptionMessage = "The Server has encountered an error.";

        public InternalServerException() : base(InternalServerExceptionMessage)
        {
        }

        public InternalServerException(string message) : base(message)
        {
        }
    }
}