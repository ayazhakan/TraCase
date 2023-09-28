using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {


        services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<DataContext>()
        .AddDefaultTokenProviders();


            services.AddScoped<IUserService, UserService>();
            var jwtSettings = Configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            services.AddAuthentication(auth =>
   {
       auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
       auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   }).AddJwtBearer(jwt =>
   {
       jwt.RequireHttpsMetadata = false;
       jwt.SaveToken = true;
       jwt.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(key),
           ValidateIssuer = false,
           ValidateAudience = false
           
       };
   });


            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
