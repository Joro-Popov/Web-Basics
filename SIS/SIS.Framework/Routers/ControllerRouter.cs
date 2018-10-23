﻿namespace SIS.Framework.Routers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ComponentModel.DataAnnotations;

    using ActionResults.Contracts.Base;
    using Attributes.Methods.Base;
    using Attributes.Action;
    using Services.Contracts;
    using ActionResults.Contracts;
    using Controllers;
    using WebServer.API;
    using WebServer.Results;
    
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using HTTP.Extensions;

    public class ControllerRouter : IHttpHandler
    {
        private const string NOT_SUPPORTED_VIEW_RESULT = "Type of result is not supported!";

        private const string DEFAULT_CONTROLLER_NAME = "Home";

        private const string DEFAULT_ACTION_NAME = "Index";
        
        private readonly IServiceCollection serviceCollection;

        public ControllerRouter(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            var controllerName = string.Empty;
            var stringAction = string.Empty;

            if (request.Path == "/")
            {
                controllerName = DEFAULT_CONTROLLER_NAME + MvcContext.Get.ControllersSuffix;
                stringAction = DEFAULT_ACTION_NAME;
            }
            else
            {
                var requestArgs = request.Path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

                controllerName = requestArgs.First().Capitalize() + MvcContext.Get.ControllersSuffix;

                stringAction = requestArgs.Last().Capitalize();
            }
            
            var controller = this.GetController(controllerName, request);
            
            var action = this.GetMethod(request.RequestMethod.ToString(), controller, stringAction);
            
            if(controller == null || action == null) throw new NullReferenceException();

            var actionParameters = this.MapActionParameters(action, request, controller);
            
            return this.Authorize(controller, action) 
                   ?? this.PrepareResponse(this.InvokeAction(controller, action, actionParameters), controller);
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

        private Controller GetController(string controllerName, IHttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(controllerName)) return null;

            var controllerTypeName =
                $"{MvcContext.Get.AssemblyName}.{MvcContext.Get.ControllersFolder}.{controllerName}, {MvcContext.Get.AssemblyName}";

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
        
        private IHttpResponse PrepareResponse(IActionResult actionResult, Controller controller)
        {
            var invocationResult = actionResult.Invoke();
            
            switch (actionResult)
            {
                case IViewable _:
                    return controller.HtmlResult(invocationResult);

                case IRedirectable _:
                    return controller.RedirectResult(invocationResult);

                default:
                    throw new InvalidOperationException(NOT_SUPPORTED_VIEW_RESULT);
            }
        }

        private IHttpResponse Authorize(Controller controller, MethodInfo action)
        {
            var hasIdentity = action.GetCustomAttributes()
                .Where(a => a is AuthorizeAttribute)
                .Cast<AuthorizeAttribute>()
                .Any(a => !a.IsAuthorized(controller.Identity));

            return hasIdentity ? new UnauthorizedResult() : null;
        }
    }
}
