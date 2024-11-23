using Taller_1_IDWM;

var builder = new Builder();
var app = await builder.Build(args);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();