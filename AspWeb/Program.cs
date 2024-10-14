using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TelecomApp.Models;
using TelecomApp.Services;
using AspWeb.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

namespace AspWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<Db8328Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TelecomDatabase")));

            builder.Services.AddMemoryCache();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ICachedDataService, CachedDataService>();

            // ���������� ��������� ������
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseSession();








            // ����� ���������� � �������
            app.Map("/info", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // ������������ ������ ��� ������ 
                    string strResponse = "<HTML><HEAD><TITLE>����������</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>����������:</H1>";
                    strResponse += "<BR> ������: " + context.Request.Host;
                    strResponse += "<BR> ����: " + context.Request.PathBase;
                    strResponse += "<BR> ��������: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>�������</A></BODY></HTML>";
                    // ����� ������
                    await context.Response.WriteAsync(strResponse);
                });
            });




            //����������� � Session ��������, ��������� � �����
            app.Map("/form", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // ���������� �� Session ������� User
                    Subscriber user = context.Session.Get<Subscriber>("user") ?? new Subscriber();

                    // ������������ ������ ��� ������ ������������ HTML �����
                    string strResponse = "<HTML><HEAD><TITLE>������������</TITLE></HEAD>" +
                        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                        "<BODY><FORM action ='/form' method='POST'>" + // ���������� '/form'
                        "���:<BR><INPUT type = 'text' name = 'FullName' value = '" + user.FullName + "'>" +
                        "<BR>�������:<BR><INPUT type = 'text' name = 'PassportData' value = '" + user.PassportData + "' >" +
                        "<BR><BR><INPUT type ='submit' value='��������� � Session'><INPUT type ='submit' value='��������'></FORM>";
                    strResponse += "<BR><A href='/'>�������</A></BODY></HTML>";

                    // �������� �� ����� �������
                    if (context.Request.Method == "POST")
                    {
                        // ������ � Session ������ ������� User
                        user.FullName = context.Request.Form["FullName"];
                        user.PassportData = context.Request.Form["PassportData"];
                        context.Session.Set<Subscriber>("user", user);
                    }

                    // ����������� ����� ������������ HTML �����
                    await context.Response.WriteAsync(strResponse);
                });
            });



            // ����������� � Cookies ��������, ��������� � �����
            app.Map("/form2", async context =>
            {
                // ���������� �� ���� ������� User
                var cookieValue = context.Request.Cookies["user"];
                Subscriber user = cookieValue != null ? JsonConvert.DeserializeObject<Subscriber>(cookieValue) : new Subscriber();

                // ������������ ������ ��� ������ ������������ HTML �����
                string strResponse = "<HTML><HEAD><TITLE>������������</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><FORM action='/form2' method='POST'>" +
                    $"���:<BR><INPUT type='text' name='FullName' value='{user.FullName}'>" +
                    $"<BR>�������:<BR><INPUT type='text' name='PassportData' value='{user.PassportData}' >" +
                    "<BR><BR><INPUT type='submit' value='��������� � Cookies'><INPUT type='submit' value='��������'></FORM>";
                strResponse += "<BR><A href='/'>�������</A></BODY></HTML>";

                // �������� �� ����� �������
                if (context.Request.Method == "POST")
                {
                    // ������ � ���� ������ ������� User
                    user.FullName = context.Request.Form["FullName"];
                    user.PassportData = context.Request.Form["PassportData"];

                    // ������������ ������� Subscriber � JSON � ���������� � ����
                    var options = new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddMinutes(30) // ������������� ���� �������� ����
                    };
                    context.Response.Cookies.Append("user", JsonConvert.SerializeObject(user), options);
                }

                // ����������� ����� ������������ HTML �����
                await context.Response.WriteAsync(strResponse);
            });

            // ����� ������������ ���������� �� ������� ���� ������
            app.Map("/tariff", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    var cachedDataService = context.RequestServices.GetService<ICachedDataService>();
                    IEnumerable<TariffPlan> tariff = cachedDataService.GetTariffPlans("Tariff20");
                    string HtmlString = "<HTML><HEAD><TITLE>������</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>������ �������</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>ID</TH>";
                    HtmlString += "<TH>��������� �������� ������</TH>";
                    HtmlString += "<TH>��������� ������������� �����</TH>";
                    HtmlString += "<TH>��������� ��������</TH>";
                    HtmlString += "</TR>";
                    foreach (var tar in tariff)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + tar.TariffPlanId + "</TD>";
                        HtmlString += "<TD>" + tar.LocalCallRate + "</TD>";
                        HtmlString += "<TD>" + tar.LongDistanceCallRate + "</TD>";
                        HtmlString += "<TD>" + tar.InternationalCallRate + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>�������</A></BR>";
                    HtmlString += "<BR><A href='/tariff'>������</A></BR>";
                    HtmlString += "<BR><A href='/form'>������ ���������</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // ����� ������
                    await context.Response.WriteAsync(HtmlString);
                });
            });










            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
