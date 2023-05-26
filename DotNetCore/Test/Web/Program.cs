using log4net;


var builder = WebApplication.CreateBuilder(args);

//注入Log4Net
builder.Services.AddLogging(cfg =>
{
    cfg.AddLog4Net();
    //默认的配置文件路径是在根目录，且文件名为log4net.config
    //如果文件路径或名称有变化，需要重新设置其路径或名称
    //比如在项目根目录下创建一个名为cfg的文件夹，将log4net.config文件移入其中，并改名为log.config
    //则需要使用下面的代码来进行配置
    //cfg.AddLog4Net(new Log4NetProviderOptions()
    //{
    //    Log4NetConfigFileName = "cfg/log.config",
    //    Watch = true
    //});
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


//访问根页面时
//app.MapGet("/", (ILogger<Program> logger) =>
//{
//    logger.LogInformation("logger：测试一下Log4Net=》Info");
//    return "Hello World!";
//});
//访问test页面时
//app.MapGet("/test", () =>
//{
//    var log = LogManager.GetLogger(typeof(Program));
//    log.Info("log：这是一条普通日志信息");
//});

Dictionary<string, string> dbConnType = new Dictionary<string, string>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Default}/{id?}");

app.Run();
