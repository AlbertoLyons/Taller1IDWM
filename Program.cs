using Taller_1_IDWM;
// Crea el builder
var builder = new Builder();
// Construye la aplicación con el archivo Builder.cs
var app = await builder.Build(args);
// En caso de que el ambiente sea de desarrollo
if (app.Environment.IsDevelopment())
{
    // Usa Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
    // Usa la página de error de desarrollo
    app.UseDeveloperExceptionPage();
}
// Usa la autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();
// Usa los controladores
app.MapControllers();
// Ejecuta el programa
app.Run();