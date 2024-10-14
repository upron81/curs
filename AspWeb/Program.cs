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

            // добавление поддержки сессии
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








            // Вывод информации о клиенте
            app.Map("/info", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Формирование строки для вывода 
                    string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Информация:</H1>";
                    strResponse += "<BR> Сервер: " + context.Request.Host;
                    strResponse += "<BR> Путь: " + context.Request.PathBase;
                    strResponse += "<BR> Протокол: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";
                    // Вывод данных
                    await context.Response.WriteAsync(strResponse);
                });
            });




            //Запоминание в Session значений, введенных в форме
            app.Map("/form", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Считывание из Session объекта User
                    Subscriber user = context.Session.Get<Subscriber>("user") ?? new Subscriber();

                    // Формирование строки для вывода динамической HTML формы
                    string strResponse = "<HTML><HEAD><TITLE>Пользователь</TITLE></HEAD>" +
                        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                        "<BODY><FORM action ='/form' method='POST'>" + // Возвращаем '/form'
                        "Имя:<BR><INPUT type = 'text' name = 'FullName' value = '" + user.FullName + "'>" +
                        "<BR>Паспорт:<BR><INPUT type = 'text' name = 'PassportData' value = '" + user.PassportData + "' >" +
                        "<BR><BR><INPUT type ='submit' value='Сохранить в Session'><INPUT type ='submit' value='Показать'></FORM>";
                    strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";

                    // Проверка на метод запроса
                    if (context.Request.Method == "POST")
                    {
                        // Запись в Session данных объекта User
                        user.FullName = context.Request.Form["FullName"];
                        user.PassportData = context.Request.Form["PassportData"];
                        context.Session.Set<Subscriber>("user", user);
                    }

                    // Асинхронный вывод динамической HTML формы
                    await context.Response.WriteAsync(strResponse);
                });
            });



            // Запоминание в Cookies значений, введенных в форме
            app.Map("/form2", async context =>
            {
                // Считывание из куки объекта User
                var cookieValue = context.Request.Cookies["user"];
                Subscriber user = cookieValue != null ? JsonConvert.DeserializeObject<Subscriber>(cookieValue) : new Subscriber();

                // Формирование строки для вывода динамической HTML формы
                string strResponse = "<HTML><HEAD><TITLE>Пользователь</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><FORM action='/form2' method='POST'>" +
                    $"Имя:<BR><INPUT type='text' name='FullName' value='{user.FullName}'>" +
                    $"<BR>Паспорт:<BR><INPUT type='text' name='PassportData' value='{user.PassportData}' >" +
                    "<BR><BR><INPUT type='submit' value='Сохранить в Cookies'><INPUT type='submit' value='Показать'></FORM>";
                strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";

                // Проверка на метод запроса
                if (context.Request.Method == "POST")
                {
                    // Запись в куки данных объекта User
                    user.FullName = context.Request.Form["FullName"];
                    user.PassportData = context.Request.Form["PassportData"];

                    // Сериализация объекта Subscriber в JSON и сохранение в куки
                    var options = new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddMinutes(30) // Устанавливаем срок действия куки
                    };
                    context.Response.Cookies.Append("user", JsonConvert.SerializeObject(user), options);
                }

                // Асинхронный вывод динамической HTML формы
                await context.Response.WriteAsync(strResponse);
            });

            // Вывод кэшированной информации из таблицы базы данных
            app.Map("/tariff", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    var cachedDataService = context.RequestServices.GetService<ICachedDataService>();
                    IEnumerable<TariffPlan> tariff = cachedDataService.GetTariffPlans("Tariff20");
                    string HtmlString = "<HTML><HEAD><TITLE>Тарифы</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список тарифов</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>ID</TH>";
                    HtmlString += "<TH>Стоимость местного звонка</TH>";
                    HtmlString += "<TH>Стоимость междугорожней связи</TH>";
                    HtmlString += "<TH>Стоимость роуминга</TH>";
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
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/tariff'>Тарифы</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные абонентов</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
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
