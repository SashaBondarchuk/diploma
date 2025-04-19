
using Employee.Performance.Evaluator.API.Extensions;
using Employee.Performance.Evaluator.API.Middlewares;
using System.Text.Json.Serialization;

namespace Employee.Performance.Evaluator.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddServices(builder.Configuration);
        builder.Services.AddAppDbContext(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            options.JsonSerializerOptions.WriteIndented = true;
        });

        builder.Services.AddCors();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(opt => opt
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAppDbContext();

        app.UseMiddleware<UserSaverMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
