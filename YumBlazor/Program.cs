using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Stripe;
using YumBlazor.Components;
using YumBlazor.Components.Account;
using YumBlazor.Data;
using YumBlazor.Repository;
using YumBlazor.Repository.IRepository;
using YumBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddRadzenComponents();
builder.Services.AddSingleton<SharedStateService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = IdentityConstants.ApplicationScheme;
		options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})//https://developers.google.com/identity/oauth2/web/guides/get-google-api-clientid

    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration.GetSection("FacebookAppId").Value!;
        options.AppSecret = builder.Configuration.GetSection("FacebookAppSecret").Value!;
    })
	.AddMicrosoftAccount(options =>
	{
		options.ClientId = builder.Configuration.GetSection("MicrosoftAppId").Value!;
		options.ClientSecret = builder.Configuration.GetSection("MicrosoftAppSecret").Value!;
	})
	.AddGoogle(options =>
    {
        options.ClientId = builder.Configuration.GetSection("GoogleAppId").Value!;
        options.ClientSecret = builder.Configuration.GetSection("GoogleAppSecret").Value!;
    })
.AddIdentityCookies();

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//	options.UseSqlServer(connectionString),ServiceLifetime.Transient);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();
StripeConfiguration.ApiKey= builder.Configuration.GetSection("StripeApiKey").Value;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
