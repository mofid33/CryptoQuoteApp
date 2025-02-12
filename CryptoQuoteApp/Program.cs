﻿using CryptoQuoteApp.Repositories;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<ICryptoService, CryptoService>();
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
        .AddXmlDataContractSerializerFormatters(); builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();

