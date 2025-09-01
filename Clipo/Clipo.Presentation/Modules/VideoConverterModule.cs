using Clipo.Application.UseCases.ConvertVideoToFrame;
using Clipo.Application.UseCases.GetVideosByUser;
using Clipo.Application.UseCases.GetVideoStatus;
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

            services.AddScoped<IGetVideosByUserOutputPort, GetVideosByUserPresenter>();
            services.AddScoped<GetVideosByUserController>();

            services.AddScoped<IGetVideoStatusOutputPort, GetVideoStatusPresenter>();
            services.AddScoped<GetVideoStatusController>();

            return services;
        }
    }
}
