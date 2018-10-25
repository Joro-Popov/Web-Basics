namespace SIS.Framework.Attributes.Methods
{
    public class HttpPutAttribute : HttpMethodAttribute
    {
        private const string REQUEST_METHOD = "PUT";
        
        public override bool IsValid(string requestMethod)
        {
            return this.IsValidMethod(requestMethod, REQUEST_METHOD);
        }
    }
}
