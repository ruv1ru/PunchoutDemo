using System;
using Microsoft.AspNetCore.Builder;

namespace PunchoutWebsite.CXML
{
    public static class CxmlMiddlewareExtensions
    {
        public static IApplicationBuilder UseCxmlMiddlewareExtensions
                                      (this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CxmlSetupHanlder>();
        }
    }
}
