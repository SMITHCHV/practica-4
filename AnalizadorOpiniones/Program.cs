var builder = WebApplication.CreateBuilder(args);

// Agregar controladores con vistas
builder.Services.AddControllersWithViews();

// Registrar el servicio de análisis de sentimiento como singleton
builder.Services.AddSingleton<AnalizadorOpiniones.Services.SentimentService>();

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

// Ruta por defecto apuntando al controlador Analisis y acción Sentimiento
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Analisis}/{action=Sentimiento}/{id?}");

app.Run();
