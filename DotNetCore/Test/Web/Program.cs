using log4net;


var builder = WebApplication.CreateBuilder(args);

//ע��Log4Net
builder.Services.AddLogging(cfg =>
{
    cfg.AddLog4Net();
    //Ĭ�ϵ������ļ�·�����ڸ�Ŀ¼�����ļ���Ϊlog4net.config
    //����ļ�·���������б仯����Ҫ����������·��������
    //��������Ŀ��Ŀ¼�´���һ����Ϊcfg���ļ��У���log4net.config�ļ��������У�������Ϊlog.config
    //����Ҫʹ������Ĵ�������������
    //cfg.AddLog4Net(new Log4NetProviderOptions()
    //{
    //    Log4NetConfigFileName = "cfg/log.config",
    //    Watch = true
    //});
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


//���ʸ�ҳ��ʱ
//app.MapGet("/", (ILogger<Program> logger) =>
//{
//    logger.LogInformation("logger������һ��Log4Net=��Info");
//    return "Hello World!";
//});
//����testҳ��ʱ
//app.MapGet("/test", () =>
//{
//    var log = LogManager.GetLogger(typeof(Program));
//    log.Info("log������һ����ͨ��־��Ϣ");
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
