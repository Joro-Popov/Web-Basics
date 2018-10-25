using SIS.Framework.Routers.Contracts;

namespace SIS.Framework.Routers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ComponentModel.DataAnnotations;

    using ActionResults.Contracts;
    using Attributes.Methods;
    using Attributes.Action;
    using Services.Contracts;
    using Controllers;
    using WebServer.API;
    using WebServer.Results;
    using SIS.HTTP.Enums;

    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using HTTP.Extensions;

    public class ControllerRouter : IMvcRouter
    {
        private const string DEFAULT_ROUTE = "/";

        private const string REQUEST_URL_CONTROLLER_ACTION_SEPARATOR = "/";

        private const string DEFAULT_CONTROLLER_NAME = "Home";

        private const string DEFAULT_ACTION_NAME = "Index";

        private const string DEFAULT_CONTROLLER_ACTION_REQUEST_METHOD = "GET";

        private const string NOT_SUPPORTED_VIEW_RESULT = "The Action result is not supported.";
        

        private readonly IServiceCollection serviceCollection;

        public ControllerRouter(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            var controllerAndActionNames = this.ExtractControllerAndActionNames(request);

            string controllerName = controllerAndActionNames[0];
            string actionName = controllerAndActionNames[1];

            var controller = this.GetController(controllerName, request);
            
            var action = this.GetMethod(request.RequestMethod.ToString(), controller, actionName);
            
            if(controller == null || action == null) return null;

            var actionParameters = this.MapActionParameters(action, request, controller);

            if (!this.IsAuthorized(controller, action))
            {
                return new UnauthorizedResult();
            }

            return this.PrepareResponse(this.InvokeAction(controller, action, actionParameters));
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

        private bool? IsValidModel(object bindingModel)
        {
            var bindingModelProperties = bindingModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var property in bindingModelProperties)
            {
                IEnumerable<ValidationAttribute> validationAttributes
                    = property.GetCustomAttributes()
                        .Where(a => a is ValidationAttribute)
                        .Cast<ValidationAttribute>()
                        .ToList();

                if (validationAttributes.Any(a => !a.IsValid(property.GetValue(bindingModel))))
                {
                    return false;
                }
            }

            return true;
        }

        private object ProcessBindingModelParameters(ParameterInfo param, IHttpRequest request)
        {
            var bindingModelType = param.ParameterType;

            var bindingModelInstance = this.serviceCollection.CreateInstance(bindingModelType);

            var bindingModelProperties = bindingModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var property in bindingModelProperties)
            {
                try
                {
                    var value = this.GetParameterFromRequestData(request, property.Name);
                    
                    property.SetValue(bindingModelInstance, Convert.ChangeType(value, property.PropertyType));
                }
                catch (Exception)
                {
                    Console.WriteLine($"The {property.Name} field could not be mapped.");
                }
            }

            return Convert.ChangeType(bindingModelInstance, bindingModelType);
        }

        private object ProcessPrimitiveParameter(IHttpRequest request, ParameterInfo paramInfo)
        {
            var value = this.GetParameterFromRequestData(request, paramInfo.Name);

            return value == null ? value : Convert.ChangeType(value, paramInfo.ParameterType);
        }

        private object GetParameterFromRequestData(IHttpRequest request, string paramName)
        {
            if (request.QueryData.Any(x => x.Key.ToLower() == paramName.ToLower()))
            {
                return request.QueryData.FirstOrDefault(x => x.Key.ToLower() == paramName.ToLower()).Value;
            }

            return request.FormData.FirstOrDefault(x => x.Key.ToLower() == paramName.ToLower()).Value;
        }

        private string[] ExtractControllerAndActionNames(IHttpRequest request)
        {
            string[] result = new string[2];

            if (request.Path == DEFAULT_ROUTE)
            {
                result[0] = DEFAULT_CONTROLLER_NAME;
                result[1] = DEFAULT_ACTION_NAME;
            }
            else
            {
                var requestUrlSplit = request.Path.Split(
                    REQUEST_URL_CONTROLLER_ACTION_SEPARATOR
                    , StringSplitOptions.RemoveEmptyEntries);

                result[0] = requestUrlSplit[0].Capitalize();
                result[1] = requestUrlSplit[1].Capitalize();
            }

            return result;
        }

        private Controller GetController(string controllerName, IHttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(controllerName)) return null;

            var controllerTypeName =
                $"{MvcContext.Get.AssemblyName}." +
                $"{MvcContext.Get.ControllersFolder}." +
                $"{controllerName}{MvcContext.Get.ControllersSuffix}, " +
                $"{MvcContext.Get.AssemblyName}";

            var controllerType = Type.GetType(controllerTypeName);

            var controller = (Controller)this.serviceCollection.CreateInstance(controllerType);

            if (controller != null) controller.Request = request;

            return controller;
        }
        
        private MethodInfo GetMethod(string requestMethod, Controller controller, string actionName)
        {
            var actions = this.GetSuitableMethods(controller, actionName);

            foreach (var suitableMethod in actions)
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

            return null; 
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
                    throw new InvalidOperationException(NOT_SUPPORTED_VIEW_RESULT);
            }
        }

        private bool IsAuthorized(Controller controller, MethodInfo action)
            => action
                .GetCustomAttributes()
                .Where(a => a is AuthorizeAttribute)
                .Cast<AuthorizeAttribute>()
                .All(a => a.IsAuthorized(controller.Identity));
    }
}
