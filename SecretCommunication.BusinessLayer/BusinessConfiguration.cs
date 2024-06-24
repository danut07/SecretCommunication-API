using Microsoft.Extensions.DependencyInjection;
using SecretCommunication.BusinessLayer.Interface;

namespace SecretCommunication.BusinessLayer
{
    public static class BusinessConfiguration
    {
        public static void RegisterBusinessLayer(this IServiceCollection services)
        {
            services.AddScoped<IImageService, ImageService>();
        }
    }
}
