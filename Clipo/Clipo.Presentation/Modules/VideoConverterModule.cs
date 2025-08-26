using Clipo.Application.UseCases.ConvertVideoToFrame;
using Clipo.Presentation.Controller.VideoConverter;
using Clipo.Presentation.Presenters.VideoConverter;
using Microsoft.Extensions.DependencyInjection;

namespace Clipo.Presentation.Modules
{
    public static class VideoConverterModule
    {
        public static IServiceCollection AddVideoConverterPresentation(this IServiceCollection services)
        {
            services.AddScoped<IConvertVideoToFrameOutputPort, ConvertVideoToFramePresenter>();

            services.AddScoped<ConvertVideoToFrameController>();

            return services;
        }
    }
}
