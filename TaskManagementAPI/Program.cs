using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Interfaces.RabbitMQ;
using TaskManagementAPI.Services;
using TaskManagementAPI.Services.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8090);
    //options.ListenLocalhost(8091, listenOptions => listenOptions.UseHttps());
});

builder.Services.AddDbContext<TaskDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddSingleton<IRabbitTaskApiReceiver, ReceiveMessageService>();
builder.Services.AddSingleton<IRabbitTaskApiSender, SendMessageService>();

var app = builder.Build();

var senderService = app.Services.GetRequiredService<IRabbitTaskApiSender>();
await senderService.InitializeSenderService();

var receiverService = app.Services.GetRequiredService<IRabbitTaskApiReceiver>();
var consumerTasks = new[]
{
    Task.Run(() => receiverService.ReceiveGetAllTaskMessageAndAnswer()),
    Task.Run(() => receiverService.ReceiveCreateTaskMessageAndAnswer()),
    Task.Run(() => receiverService.ReceiveUpdateTaskMessageAndAnswer()),
    Task.Run(() => receiverService.ReceiveDeleteTaskMessageAndAnswer())
};
await Task.WhenAll(consumerTasks);

app.Lifetime.ApplicationStopping.Register(async () =>
{
    var receiver = app.Services.GetRequiredService<IRabbitTaskApiReceiver>() as IAsyncDisposable;
    if (receiver != null)
    {
        await receiver.DisposeAsync();
    }

    var sender = app.Services.GetRequiredService<IRabbitTaskApiSender>() as IAsyncDisposable;
    if (sender != null)
    {
        await sender.DisposeAsync();
    }
});

app.UseHttpsRedirection();

app.Run();
