using ProcessServices.Services.Processor.Bog;
using ProcessServices.Services.Processor.Bog.Impl;
using ProcessServices.Services.Processor.Danger;
using ProcessServices.Services.Processor.Danger.Impl;
using ProcessServices.Services.Processor.Vpcs;
using ProcessServices.Services.Processor.Vpcs.Impl;
using ProcessServices.Services.Uploader;
using ProcessServices.Services.Uploader.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFileUpload,ExcelFileUpload>();
builder.Services.AddScoped<IDataProcess, VPCsExcelDataProcess>();
builder.Services.AddScoped<IDangerDataProcess, DangerExcelDataProcess>();
builder.Services.AddScoped<IBogDataProcess  , BogExcelDataProcess>();
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
