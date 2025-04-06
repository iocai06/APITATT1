using APITATT1.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Banco de dados
builder.Services.AddDbContext<Contexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexao")));

// CORS — definir ANTES do build
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("CorsPolicy");         // 1️⃣ CORS primeiro
app.UseHttpsRedirection();         // 2️⃣ HTTPS redirecionamento
app.UseAuthorization();            // 3️⃣ Autorização
app.MapControllers();              // 4️⃣ Mapeia os endpoints

app.Run();
