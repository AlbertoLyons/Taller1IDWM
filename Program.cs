using Taller_1_IDWM;

var builder = new Builder();
var app = builder.Build(args);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.MapControllers();
app.Run();