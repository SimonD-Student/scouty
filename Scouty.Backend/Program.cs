using Microsoft.EntityFrameworkCore;
using Scouty.Backend.Data;
using Scouty.Backend.Extensions; 

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATION DES SERVICES (DÉPENDANCES) ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocumentation();

// Configuration DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ScoutyDbContext>(options =>
    options.UseNpgsql(connectionString));

// Appel de notre extension Identity/JWT (Code déplacé)
// On passe 'builder.Configuration' car l'extension a besoin de lire appsettings.json
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Ce bloc vérifie si la DB existe. Sinon, il la crée et applique le schéma.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ScoutyDbContext>();

        // Log pour qu'on puisse voir ça dans les logs Docker (commande: docker logs)
        Console.WriteLine(" Vérification des migrations DB...");

        // Applique les migrations en attente
        context.Database.Migrate();

        Console.WriteLine(" Base de données prête et à jour");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"[ERREUR CRITIQUE] Impossible de migrer la DB : {ex.Message}");
        // En prod, on pourrait vouloir arrêter l'app ici si la DB est HS, 
        // mais pour l'instant on laisse continuer pour voir l'erreur.
    }
}
// =======================================================================

// --- 2. PIPELINE HTTP ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Qui es-tu ?
app.UseAuthorization();  // As-tu le droit ?

app.MapControllers();

app.Run();