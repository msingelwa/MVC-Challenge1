using MVC_Challenge.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddSingleton<IInvoiceRepository, DBLayer>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Invoice}/{action=Index}");
app.MapControllerRoute(
    name: "GetInvoice",
    defaults: new { controller = "Invoice", action = "Index" },
    pattern: "GetInvoice/{id}");
app.MapControllerRoute(
    name: "ListOfInvoice",
    defaults: new { controller = "Invoice", action = "Index" },
    pattern: "{controller}/{action}/{id?}");

app.Run();
