using Cyh.Net;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ImgRepo.Web.StreamFileHelper
{
    public class GenerateAntiforgeryTokenCookieAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            IAntiforgery? antiforgery = context.HttpContext.RequestServices.GetService<IAntiforgery>();
            if (antiforgery == null) return;
            // Send the request token as a JavaScript-readable cookie
            AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
            if (tokens.RequestToken.IsNullOrEmpty()) return;
            context.HttpContext.Response.Cookies.Append(
                "RequestVerificationToken",
                tokens.RequestToken,
                new CookieOptions() { HttpOnly = false });
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
