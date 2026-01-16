using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // <--- NOUVEAU (Pour la crypto)
using Scouty.Backend.Data;
using Scouty.Backend.Entities;
using Scouty.Shared.DTOs;
using System.IdentityModel.Tokens.Jwt; // <--- NOUVEAU (Pour le JWT)
using System.Security.Claims; // <--- NOUVEAU (Pour les infos dans le token)
using System.Text; // <--- NOUVEAU (Pour l'encodage)

namespace Scouty.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ScoutyDbContext _context;
    private readonly IConfiguration _configuration; // <--- NOUVEAU : Pour lire appsettings.json

    // Injection de la DB ET de la Configuration
    public AuthController(ScoutyDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register(RegisterRequestDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Cet email est déjà utilisé.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new UserEntity
        {
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Totem = request.Totem,
            Email = request.Email,
            PasswordHash = passwordHash,
            Section = request.Section,
            IsChef = request.IsChef,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        // --- CHANGEMENT ICI ---
        // On génère le vrai token au lieu du texte bidon
        string token = CreateToken(newUser);

        return Ok(new LoginResponseDto
        {
            UserEmail = newUser.Email,
            UserName = newUser.Totem,
            Token = token, // Le vrai JWT
            IsChef = newUser.IsChef

        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized("Email ou mot de passe incorrect.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Email ou mot de passe incorrect.");
        }

        // --- CHANGEMENT ICI ---
        string token = CreateToken(user);

        return Ok(new LoginResponseDto
        {
            UserEmail = user.Email,
            UserName = user.Totem,
            Token = token, // <--- Le vrai JWT
            IsChef = user.IsChef
        });
    }

    // --- NOUVELLE MÉTHODE PRIVÉE ---
    private string CreateToken(UserEntity user)
    {
        // 1. On définit les "Claims" (Les infos qu'on veut stocker DANS le jeton)
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // L'ID user
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Firstname),
            new Claim("IsChef", user.IsChef.ToString()), // Notre info perso
            new Claim("Section", user.Section.ToString())
        };

        // 2. On récupère la clé secrète depuis appsettings.json
        // Attention : Il faut que la clé existe dans le fichier JSON !
        var keyString = _configuration.GetSection("JwtSettings:Key").Value;
        if (string.IsNullOrEmpty(keyString)) throw new Exception("Clé JWT non configurée dans appsettings.json");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

        // 3. On signe le token (pour empêcher la falsification)
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // 4. On crée l'objet Token
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(30), // Valide 30 jours
            signingCredentials: creds,
            issuer: _configuration.GetSection("JwtSettings:Issuer").Value,
            audience: _configuration.GetSection("JwtSettings:Audience").Value
        );

        // 5. On le transforme en string
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}