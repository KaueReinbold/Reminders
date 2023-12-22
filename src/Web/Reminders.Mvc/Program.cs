var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .RegisterApplicationServices(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        SupportedDatabases.SqlServer)
    .AddControllersWithViews()
    .AddApplicationValidations(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting()
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Reminders}/{action=Index}/{id?}");
    });

app.MigrateRemindersDatabase();

app.Run();
