using ProcessServices.Services.Processor;
using ProcessServices.Services.Processor.Impl;
using ProcessServices.Services.Uploader;
using ProcessServices.Services.Uploader.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFileUpload,ExcelFileUpload>();
builder.Services.AddScoped<IDataProcess, ExcelDataProcess>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            
            policy.WithOrigins(builder.Configuration.GetSection("ALLOWED_ORIGIN:ALLOWED_DEV_ORIGIN").Value)
            .AllowAnyMethod()
            .AllowAnyHeader();
                  
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();
