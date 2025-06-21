// No seu arquivo Program.cs

using APITATT1.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // Já deve estar aí, mas certifique-se
using Microsoft.Extensions.DependencyInjection; // Para o bloco de seeding
using Microsoft.Extensions.Logging;             // Para o bloco de seeding

var builder = WebApplication.CreateBuilder(args);

// Banco de dados
builder.Services.AddDbContext<Contexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexao")));

// CORS
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

// --- ADICIONE A CONFIGURAÇÃO DE AUTENTICAÇÃO JWT AQUI ---
// (Isso deve vir antes de AddEndpointsApiExplorer e AddSwaggerGen)

// Pegar a chave secreta do appsettings.json
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

// Configurar a autenticação JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Apenas para desenvolvimento (mude para true em produção!)
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
// --- FIM DA CONFIGURAÇÃO DE AUTENTICAÇÃO JWT ---


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Define informações básicas da API no Swagger
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "APITATT1", Version = "v1" });

    // --- Adiciona a definição de segurança JWT para o Swagger UI ---
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT neste formato: Bearer SEUTOKEN",
    });

    // Adiciona o requisito de segurança globalmente para todas as operações
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    // --- Fim da definição de segurança JWT para o Swagger UI ---
});


var app = builder.Build();

// --- Bloco para criar um usuário inicial se não existir ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Contexto>();
        context.Database.Migrate();

        if (!context.Users.Any())
        {
            var initialUser = new User
            {
                Username = "adminapi",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("suasenhasecreta")
            };
            context.Users.Add(initialUser);
            context.SaveChanges();
            Console.WriteLine("Usuário 'adminapi' criado com sucesso!");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao seedar o banco de dados.");
    }
}
// --- Fim do bloco de criação de usuário inicial ---


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

// --- ATENÇÃO À ORDEM AQUI! UseAuthentication DEVE VIR ANTES de UseAuthorization ---
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();