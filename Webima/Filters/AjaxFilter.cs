using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Webima.Filters
{
    public class AjaxFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
