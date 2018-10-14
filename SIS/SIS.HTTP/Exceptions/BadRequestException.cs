namespace SIS.HTTP.Exceptions
{
    using System;

    public class BadRequestException : Exception
    {
        public const string BadRequestExceptionMessage = "The Request was malformed or contains unsupported elements.";

        public BadRequestException() : base(BadRequestExceptionMessage)
        {
        }

        public BadRequestException(string message) : base(message)
        {
        }
    }
}