var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("CookieAuth")
        .AddCookie("CookieAuth", config => {
            config.Cookie.Name = "User.Cookie";
            config.ExpireTimeSpan = TimeSpan.FromDays(30);
            config.LoginPath = "/fiok/bejelentkezes";
        });
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromDays(30);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<Auction_house.StateHub>("/stateHub");
});

app.MapControllers();

app.Run();
