var builder = WebApplication.CreateBuilder(args);

// Agrega controladores con vistas
builder.Services.AddControllersWithViews();

// Servicios de ML registrados como singleton
builder.Services.AddSingleton<AnalizadorOpiniones.Services.SentimentService>();
builder.Services.AddSingleton<AnalizadorOpiniones.Services.ProductRecommendationService>();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Ruta por defecto (Home/Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
