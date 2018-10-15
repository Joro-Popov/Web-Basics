namespace SIS.Framework.Routers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ComponentModel.DataAnnotations;

    using ActionResults.Contracts;
    using Attributes.Methods;
    using Controllers;
    using HTTP.Enums;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using HTTP.Extensions;
    using WebServer.API;
    using WebServer.Results;

    public class ControllerRouter : IHttpHandler
    {
        private const string NotSupportedViewResult = "The view result is not supported!";

        public IHttpResponse Handle(IHttpRequest request)
        {
            var requestArgs = request.Path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

            var controllerName = requestArgs.First().Capitalize() + MvcContext.Get.ControllersSuffix;
            var stringAction = requestArgs.Last().Capitalize();

            var controller = this.GetController(controllerName, request);
            var action = this.GetMethod(request.RequestMethod.ToString(), controller, stringAction);
            
            var actionParameters = this.MapActionParameters(action, request, controller);
            var actionResult = this.InvokeAction(controller, action, actionParameters);

            var response = this.PrepareResponse(actionResult);

            return response;
        }

        private IActionResult InvokeAction(Controller controller, MethodInfo action, object[] actionParameters)
        {
            return (IActionResult)action.Invoke(controller, actionParameters);
        }

        private object[] MapActionParameters(MethodInfo action, IHttpRequest request, Controller controller)
        {
            var actionParametersInfo = action.GetParameters();
            var mappedActionParameters = new object[actionParametersInfo.Length];

            for (var index = 0; index < actionParametersInfo.Length; index++)
            {
                var parameterType = actionParametersInfo[index];

                if (parameterType.ParameterType.IsPrimitive || parameterType.ParameterType == typeof(string))
                {
                    mappedActionParameters[index] = ProcessPrimitiveParameter(request, parameterType);
                }
                else
                {
                    var bindingModel = ProcessBindingModelParameters(parameterType, request);
                    controller.ModelState.IsValid = this.IsValidModel(bindingModel);

                    mappedActionParameters[index] = bindingModel;
                }
            }

            return mappedActionParameters;
        }

        private bool IsValidModel(object bindingModel)
        {
            var bindingModelProperties = bindingModel.GetType().GetProperties();
            
            foreach (var bindingModelProperty in bindingModelProperties)
            {
                var validationAttributes = bindingModelProperty.GetCustomAttributes()
                    .Where(attr => attr is ValidationAttribute)
                    .Cast<ValidationAttribute>()
                    .ToList();
                
                foreach (var validationAttribute in validationAttributes)
                {
                    if (!validationAttribute.IsValid(bindingModelProperty.GetValue(bindingModel))) return false;
                }
            }

            return true;
        }

        private object ProcessBindingModelParameters(ParameterInfo param, IHttpRequest request)
        {
            var bindingModelType = param.ParameterType;

            var bindingModelInstance = Activator.CreateInstance(bindingModelType);
            var bindingModelProperties = bindingModelType.GetProperties();

            foreach (var property in bindingModelProperties)
            {
                try
                {
                    var value = this.GetParameterFromRequestData(request, property.Name);
                    property.SetValue(bindingModelInstance, Convert.ChangeType(value, property.PropertyType));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"The {property.Name} field could not be mapped.");
                }
            }

            return Convert.ChangeType(bindingModelInstance, bindingModelType);
        }

        private object ProcessPrimitiveParameter(IHttpRequest request, ParameterInfo paramInfo)
        {
            var value = this.GetParameterFromRequestData(request, paramInfo.Name);
            return Convert.ChangeType(value, paramInfo.ParameterType);
        }

        private object GetParameterFromRequestData(IHttpRequest request, string paramName)
        {
            if (request.QueryData.ContainsKey(paramName)) return request.QueryData[paramName];

            return request.FormData.ContainsKey(paramName) ? request.FormData[paramName] : null;
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
        
        private IHttpResponse PrepareResponse(IActionResult actionResult)
        {
            var invocationResult = actionResult.Invoke();

            switch (actionResult)
            {
                case IViewable _:
                    return new HtmlResult(invocationResult, HttpResponseStatusCode.Ok);
                case IRedirectable _:
                    return new RedirectResult(invocationResult);
                default:
                    throw new InvalidOperationException(NotSupportedViewResult);
            }
        }
    }
}
