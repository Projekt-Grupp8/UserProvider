using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UserProvider.Filters;

[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var config = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var secret = config?["ApiKey:Secret"];

            if (!string.IsNullOrEmpty(secret) && context.HttpContext.Request.Query.TryGetValue("key", out var key))
            {
                if (!string.IsNullOrEmpty(key) && secret == key)
                {
                    await next();
                    return;
                }
                context.Result = new UnauthorizedResult();
            }
            context.Result = new UnauthorizedResult();
        }
        catch (Exception)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
