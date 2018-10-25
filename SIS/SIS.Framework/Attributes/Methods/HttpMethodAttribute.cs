namespace SIS.Framework.Attributes.Methods
{
    using System;

    public abstract class HttpMethodAttribute : Attribute
    {
        protected virtual bool IsValidMethod(string requestMethod, string attributeName)
        {
            return requestMethod.ToUpper() == attributeName;
        }

        public abstract bool IsValid(string requestMethod);
    }
}
