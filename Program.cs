using AutoMapper;
using DotNetEnv;
using finance_management.Commands.CategorizeSingleTransaction;
using finance_management.Database;
using finance_management.Interfaces;
using finance_management.Models;
using finance_management.Repository;
using finance_management.Services;
using finance_management.Validations.Log;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Reflection;
using System.Text.Json.Serialization;


Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<SplitTransactionRequestValidator>();
        fv.DisableDataAnnotationsValidation = true;
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); ;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var connectionString = Environment.GetEnvironmentVariable("PFM_DB")
    ?? throw new InvalidOperationException("PFM_DB nije postavljen u .env");

builder.Services.AddDbContext<PfmDbContext>(options =>
    options.UseNpgsql(connectionString)
);


builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<SplitTransactionRequestValidator>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>(); builder.Services.AddScoped<CsvProcessingService>();
builder.Services.AddScoped<CsvValidationService>();
builder.Services.AddScoped<ErrorLoggingService>();
builder.Services.AddScoped<CategorizeTransactionCommandHandler>();
builder.Services.AddScoped<CategoryErrorLoggingService>();
builder.Services.AddScoped<SpendingAnalyticsErrorLoggingService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
