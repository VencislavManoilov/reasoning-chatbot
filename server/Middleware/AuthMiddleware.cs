using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using App.Data;

public class UseAuthenticationMiddlewareAttribute : TypeFilterAttribute
{
    public UseAuthenticationMiddlewareAttribute() : base(typeof(AuthenticationMiddlewareFilter))
    {
    }

    private class AuthenticationMiddlewareFilter : IAsyncActionFilter
    {
        private readonly AppDbContext _data;

        public AuthenticationMiddlewareFilter(AppDbContext data)
        {
            _data = data;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedObjectResult(new { error = "User not logged in" });
                return;
            }

            var user = await _data.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                context.Result = new NotFoundObjectResult(new { error = "User not found" });
                return;
            }

            context.HttpContext.Items["User"] = user;

            await next();
        }
    }
}