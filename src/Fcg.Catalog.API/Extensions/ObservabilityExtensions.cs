using Serilog;

namespace Fcg.Catalog.API.Extensions
{
    public static class ObservabilityExtensions
    {
        public static WebApplicationBuilder AddObservabilityExtension(this WebApplicationBuilder builder)
        {
            builder.AddSerilogExtension();
            return builder;
        }     

        #region Serilog 
        private static WebApplicationBuilder AddSerilogExtension(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            return builder;
        }
        #endregion
    }
}
