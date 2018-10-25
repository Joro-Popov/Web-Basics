namespace SIS.Framework.Attributes.Methods
{
    public class HttpGetAttribute : HttpMethodAttribute
    {
        private const string REQUEST_METHOD = "GET";

        public override bool IsValid(string requestMethod)
        {
            return this.IsValidMethod(requestMethod, REQUEST_METHOD);
        }
    }
}
