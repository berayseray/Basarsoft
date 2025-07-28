using Microsoft.EntityFrameworkCore;
using WebApplication6.Data;
using WebApplication6.Services;
using WebApplication6.Repositories;
using WebApplication6.Interfaces;
var builder = WebApplication.CreateBuilder(args);


// Bir CORS politikasý adý tanýmlandý
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Servisleri eklendi
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // React uygulama adresi
                          policy.WithOrigins("http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseNetTopologySuite()
    )
);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISpatialFeatureService, SpatialFeatureService>();
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build(); 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); // Bu satýr genellikle örtülü olarak vardýr, ama sýrayý görmek için önemli

app.UseCors(MyAllowSpecificOrigins); // <-- CORS'u burada etkinleþtirin

app.UseAuthorization();

app.MapControllers();

app.Run();