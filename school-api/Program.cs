using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using school_api.Data;
using school_api.Data.Services;
using school_api.Interfaces;
using Microsoft.AspNetCore.Identity;
using school_api.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


using school_api.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Access configuration
IConfiguration configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddSingleton(configuration);
var ConnectionString = configuration.GetValue<string>("ConnectionStrings:mySql");
string policyName = "_myAllowSpecificOrigins";
builder.Services.AddControllers();
//Configure DBContext with SQL
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(ConnectionString, new MySqlServerVersion("8.0.0")));


builder.Services.AddTransient<LogsService>();
builder.Services.AddTransient<IProfileService, ProfileService>();
builder.Services.AddTransient<IDashboardService, DashboardService>();
builder.Services.AddTransient<IAdminstratorService, AdmistrationsService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IGameService, GameService>();


//Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
 //Add JWT Bearer
.AddJwtBearer(options =>
 {
     options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetValue<string>("JWT:Secret"))),

                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetValue<string>("JWT:Issuer"),

                    ValidateAudience = true,
                    ValidAudience = configuration.GetValue<string>("JWT:Audience")
                };
            });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "School.API", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
                      builder =>
                      {
                          builder
                            .WithOrigins("http://localhost:5173/") // specifying the allowed origin
                                                                   //.WithOrigins("http://160.119.253.205:81/") // specifying the allowed origin
                                                                   //.WithOrigins("https://160.119.253.205:444/") // specifying the allowed origin
                                                                   //.WithMethods("GET","POST","PUT","DELETE","") // defining the allowed HTTP method
                            .AllowAnyMethod().AllowAnyOrigin()
                            .AllowAnyHeader(); // allowing any header to be sent
                      });
});
        

var app = builder.Build();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();
app.UseRouting();

app.UseCors(policyName);
//Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();


//Exception Handling
app.ConfigureBuildInExceptionHandler(loggerFactory);

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});
app.MapControllers();
//AppDbInitializer.Seed(app);
AppDbInitializer.SeedRoles(app).Wait();

app.Run();
