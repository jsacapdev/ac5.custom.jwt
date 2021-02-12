using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CustomJwtApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CustomJwtApi", Version = "v1" });
            });

            services.AddSingleton<SomeValidationContext>(new SomeValidationContext() { Id = 1 });

            services.AddHttpContextAccessor();

            services.AddSingleton<IAuthorizationHandler, BearerAuthorizationHandler>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer();

            services.AddAuthorization(options =>
                    options.AddPolicy("Bearer", policy => policy.AddRequirements(new BearerRequirement()))
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomJwtApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class BearerRequirement : IAuthorizationRequirement
    {
        public async Task<bool> IsTokenValid(SomeValidationContext context, string token)
        {
            await Task.Run(() => { });

            var handler = new JwtSecurityTokenHandler();

            var jsonToken = handler.ReadToken(token);

            var jwtSecurityTokens = handler.ReadToken(token) as JwtSecurityToken;

            // here you can check if the token received is valid 
            return true;
        }
    }

    public class BearerAuthorizationHandler : AuthorizationHandler<BearerRequirement>
    {
        public readonly SomeValidationContext _thatYouCanInject;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerAuthorizationHandler(IHttpContextAccessor httpContextAccessor, SomeValidationContext thatYouCanInject)
        {
            _thatYouCanInject = thatYouCanInject;

            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BearerRequirement requirement)
        {
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.Contains("Bearer"))
            {
                var token = authHeader.Replace("Bearer ", string.Empty);

                if (await requirement.IsTokenValid(_thatYouCanInject, token))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    public class SomeValidationContext
    {
        public long Id { get; set; }
    }
}
