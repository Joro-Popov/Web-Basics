﻿namespace SIS.Framework.Attributes.Methods
{
    public class HttpPostAttribute : HttpMethodAttribute
    {
        private const string REQUEST_METHOD = "POST";
        
        public override bool IsValid(string requestMethod)
        {
            return this.IsValidMethod(requestMethod, REQUEST_METHOD);
        }
    }
}
