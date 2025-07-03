using Azure.Storage.Blobs;
using AzureBlobFileUpload.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using static AzureBlobFileUpload.Enums.Constants;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration[Configurations.SQLConnectionString.ToString()];
//var blobConnectionString = builder.Configuration[Configurations.BlobConnectionString.ToString()];
Console.WriteLine(connectionString);
builder.Services.AddDbContext<FileUploadDataContext>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string 'Electronics_Parts_APIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton(option => new BlobServiceClient(blobConnectionString));

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
