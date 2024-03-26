using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildyBackend.Core.Filters
{
    /// <summary>
    /// Clase 114: https://www.udemy.com/course/desarrollando-aplicaciones-en-react-y-aspnet-core/learn/lecture/25990184#overview
    /// </summary>
    public class BadRequestParse : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var resultCast = context.Result as IStatusCodeHttpResult;
            if (resultCast == null)
            {
                return;
            }
            var statusCode = resultCast.StatusCode;
            if (statusCode == 400)
            {
                var response = new List<string>();
                var resultActual = context.Result as BadRequestObjectResult;
                if (resultActual.Value is string)
                {
                    response.Add(resultActual.Value.ToString());
                }
                else
                {
                    foreach (var key in context.ModelState.Keys)
                    {
                        foreach (var error in context.ModelState[key].Errors)
                        {
                            response.Add($"{key} :{error.ErrorMessage}");
                        }
                    }
                }
                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

    }
}