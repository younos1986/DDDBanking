using CoreOcelotApiGateway.Infrastructure.Authorizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreOcelotApiGateway.Infrastructure.AuthorizationHandler
{
    public class DynamicPermissionsAuthorizationHandler : AuthorizationHandler<DynamicPermissionRequirement>
    {
        readonly CustomCoreOcelotAuthorizer _CustomCoreOcelotAuthorizer;
        public DynamicPermissionsAuthorizationHandler(CustomCoreOcelotAuthorizer customCoreOcelotConfig)
        {
            _CustomCoreOcelotAuthorizer = customCoreOcelotConfig;
        }

        protected override Task HandleRequirementAsync(
             AuthorizationHandlerContext context,
             DynamicPermissionRequirement requirement)
        {

            var mvcContext = context.Resource as AuthorizationFilterContext;
            if (mvcContext == null)
            {
                return Task.CompletedTask;
            }

            var actionDescriptor = mvcContext.ActionDescriptor;

            var area = string.Empty;
            if (actionDescriptor.RouteValues.ContainsKey("area"))
                area = actionDescriptor.RouteValues["area"];

            var controller = actionDescriptor.RouteValues["controller"];
            var action = actionDescriptor.RouteValues["action"];


            var hasAccess = _CustomCoreOcelotAuthorizer.PerformAuthorize(context.User.Claims, area, controller, action);
            if (hasAccess)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }

    public class DynamicPermissionRequirement : IAuthorizationRequirement
    {
    }
}
