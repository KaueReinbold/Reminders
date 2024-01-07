var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
    setup.SwaggerDoc("v1", new OpenApiInfo { Title = "Reminders API", Version = "v1" }));

// Add CORS services
builder.Services.AddRemindersCors(
    builder.Configuration);

// App
builder.Services
    .RegisterApplicationServices(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        builder.Configuration.GetValue<SupportedDatabases?>("DatabaseProvider") ?? SupportedDatabases.SqlServer)
    .AddApplicationValidations()
    .AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.http://localhost:3000/
if (app.Environment.IsDevelopment())
{
    app
        .UseSwagger()
        .UseSwaggerUI(setup =>
            setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Reminders API V1"));
}

app.UseRemindersCors();

app.UseMachineNameLogging<Program>();

app.MapHealthChecks("/health");

app
    .UseRemindersExceptionHandler()
    .UseHttpsRedirection()
    .UseRouting()
    .UseAuthorization()
    .UseEndpoints(endpoints => endpoints.MapControllers());

app.MigrateRemindersDatabase();

app.Run();
