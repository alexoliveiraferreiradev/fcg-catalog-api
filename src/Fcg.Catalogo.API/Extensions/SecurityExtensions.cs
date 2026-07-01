namespace Fcg.Catalogo.API.Extensions
{
    public static class SecurityExtensions
    {
        public static WebApplication UseSecurityMiddlewares(this WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
