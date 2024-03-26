using Microsoft.AspNetCore.Mvc;

namespace BuildyBackend.Core.ApiBehavior
{
    public static class BehaviorBadRequests
    {
        public static void Parse(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var response = new List<string>();
                foreach (var key in actionContext.ModelState.Keys)
                {
                    foreach (var error in actionContext.ModelState[key].Errors)
                    {
                        response.Add($"{key} :{error.ErrorMessage}");
                    }
                }
                return new BadRequestObjectResult(response);
            };
        }

    }
}