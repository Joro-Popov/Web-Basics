namespace SIS.Framework.Attributes.Methods
{
    using Base;

    public class HttpGetAttribute : HttpMethodAttribute
    {
        private const string REQUEST_METHOD = "GET";

        public override bool IsValid(string requestMethod)
        {
            return requestMethod.ToUpper() == REQUEST_METHOD;
        }
        
    }
}
