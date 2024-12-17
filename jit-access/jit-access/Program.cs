using Hangfire;
using jit_access;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("YourConnectionString")));


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApproverPolicy", policy =>
    {
        policy.RequireRole("Approver");
    });
});

builder.Services.AddHangfire(configuration => configuration
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("YourConnectionString")));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<PermissionRevocationJob>();
BackgroundJob.Schedule(() => BackgroundJob.Enqueue<PermissionRevocationJob>(), TimeSpan.FromMinutes(10));


builder.Logging.AddConsole();
builder.Logging.AddDebug();

