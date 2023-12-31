var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddHealthChecks()
    .AddCheck<RemindersServiceHealthCheck>(nameof(RemindersServiceHealthCheck));

var apiOptionsSection = builder.Configuration.GetSection(ApiOptions.ApiOptionsSectionName);

builder.Services
    .AddHttpClient<IRemindersService, RemindersService>(nameof(RemindersService), client =>
    {
        client.BaseAddress = apiOptionsSection?.Get<ApiOptions>()?.BaseUrl ?? default;
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    });

builder.Services
    .Configure<ApiOptions>(apiOptionsSection)
    .AddControllersWithViews();

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

app.UseMachineNameLogging<Program>();

app
    .ConfigureExceptionHandler()
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

app.Run();
