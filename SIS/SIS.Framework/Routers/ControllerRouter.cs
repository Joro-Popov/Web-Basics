namespace SIS.Framework.Routers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ActionResults.Contracts;
    using Attributes.Methods;
    using Controllers;
    using HTTP.Enums;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using WebServer.API;
    using WebServer.Results;

    public class ControllerRouter : IHttpHandler
    {
        private const string NotSupportedViewResult = "The view result is not supported!";

        public IHttpResponse Handle(IHttpRequest request)
        {
            var requestArgs = request.Path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

            var controllerName = requestArgs.First() + MvcContext.Get.ControllersSuffix;
            var stringAction = requestArgs.Last();

            var controller = this.GetController(controllerName, request);
            var action = this.GetMethod(request.RequestMethod.ToString(), controller, stringAction);

            var response = this.PrepareResponse(controller, action);

            return response;
        }
 
        private Controller GetController(string controllerName, IHttpRequest request)
        {
            if (controllerName == null) return null;

            var controllerTypeName =
                $"{MvcContext.Get.AssemblyName}.{MvcContext.Get.ControllersFolder}.{controllerName}, {MvcContext.Get.AssemblyName}";

            var controllerType = Type.GetType(controllerTypeName);

            var controller = (Controller)Activator.CreateInstance(controllerType);

            if (controller != null) controller.Request = request;

            return controller;
        }
        
        private MethodInfo GetMethod(string requestMethod, Controller controller, string actionName)
        {
            MethodInfo method = null;

            foreach (var suitableMethod in this.GetSuitableMethods(controller, actionName))
            {
                var attributes = suitableMethod
                    .GetCustomAttributes()
                    .Where(attr => attr is HttpMethodAttribute)
                    .Cast<HttpMethodAttribute>()
                    .ToList();

                if (!attributes.Any() && requestMethod.ToUpper() == "GET") return suitableMethod;

                if (attributes.Any(httpMethodAttribute => httpMethodAttribute.IsValid(requestMethod)))
                {
                    return suitableMethod;
                }
            }

            return method;
        }
        
        private IEnumerable<MethodInfo> GetSuitableMethods(Controller controller, string actionName)
        {
            return controller == null ? 
                new MethodInfo[0] : 
                controller.GetType().GetMethods().Where(method => method.Name.ToLower().Equals(actionName.ToLower()));
        }
        
        private IHttpResponse PrepareResponse(Controller controller, MethodInfo action)
        {
            var actionResult =(IActionResult)action.Invoke(controller, null); 
            var invokeResult = actionResult.Invoke();

            switch (actionResult)
            {
                case IViewable _: return new HtmlResult(invokeResult, HttpResponseStatusCode.Ok);

                case IRedirectable _: return new RedirectResult(invokeResult);

                default: throw new InvalidOperationException(NotSupportedViewResult);
            }
        }
    }
}
