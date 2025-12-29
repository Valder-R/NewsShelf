using BusinessLogic.Contracts;
using BusinessLogic.Mappers;
using BusinessLogic.Services;
using DataAccess.AppDbContext;
using Microsoft.EntityFrameworkCore;
using NewsServiceNs = NewsService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(NewsMappingProfile));
builder.Services.AddScoped<IImageMapper, ImageMapper>();

builder.Services.AddScoped<INewsService, BusinessLogic.Services.NewsService>();
builder.Services.AddScoped<INewsSearchService, NewsSearchService>();
builder.Services.AddSingleton<NewsServiceNs.IRabbitMqService, NewsServiceNs.RabbitMqService>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();
