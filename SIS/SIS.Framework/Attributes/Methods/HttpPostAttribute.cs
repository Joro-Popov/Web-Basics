namespace SIS.Framework.Attributes.Methods
{
    using Base;

    public class HttpPostAttribute : HttpMethodAttribute
    {
        private const string REQUEST_METHOD = "POST";
        
        public override bool IsValid(string requestMethod)
        {
            return requestMethod.ToUpper() == REQUEST_METHOD;
        }
    }
}
