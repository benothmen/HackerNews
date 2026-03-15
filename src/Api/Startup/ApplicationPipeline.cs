using Scalar.AspNetCore;

namespace Api.Startup;

public static class ApplicationPipeline
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTheme(ScalarTheme.BluePlanet);
                options.WithTitle(typeof(Program).Assembly.GetName().Name ?? "API");
            });
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        return app;
    }
}
