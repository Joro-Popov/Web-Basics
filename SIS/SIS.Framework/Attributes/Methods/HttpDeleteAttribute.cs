namespace SIS.Framework.Attributes.Methods
{
    public abstract class HttpDeleteAttribute : HttpMethodAttribute
    {
        private const string HttpDeleteAttributeRequestMethod = "DELETE";

        public override bool IsValid(string requestMethod)
        {
            return this.IsValidMethod(requestMethod, HttpDeleteAttributeRequestMethod);
        }
    }
}
