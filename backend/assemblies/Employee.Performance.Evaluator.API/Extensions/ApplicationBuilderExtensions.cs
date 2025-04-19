using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Employee.Performance.Evaluator.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseAppDbContext(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        using var context = scope?.ServiceProvider.GetRequiredService<AppDbContext>();

        context?.Database.Migrate();
    }
}
