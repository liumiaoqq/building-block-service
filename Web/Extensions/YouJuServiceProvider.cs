using Microsoft.AspNetCore.Http;
using System;
namespace Web
{
    public static class YouJuServiceProvider
    {
        public static IServiceProvider ServiceProvider;

        public static HttpContext GetContext()
        {
            if (ServiceProvider == null) return null;
            object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));

            HttpContext context = ((IHttpContextAccessor)factory).HttpContext;

            return context;
        }

    }
}
