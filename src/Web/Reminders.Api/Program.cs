var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
    setup.SwaggerDoc("v1", new OpenApiInfo { Title = "Reminders API", Version = "v1" }));

// App
builder.Services
    .RegisterApplicationServices(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        SupportedDatabases.SqlServer)
    .AddControllers()
    .AddApplicationValidations(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app
        .UseSwagger()
        .UseSwaggerUI(setup =>
            setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Reminders API V1"));
}

app
    .UseRemindersExceptionHandler()
    .UseHttpsRedirection()
    .UseRouting()
    .UseAuthorization()
    .UseEndpoints(endpoints => endpoints.MapControllers());



try
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using var scope = scopedFactory.CreateScope();
    
    scope.ServiceProvider.GetService<RemindersContext>().Database.Migrate();
}
catch // TODO: implement the better way to migrate the database.
{ }

app.Run();
