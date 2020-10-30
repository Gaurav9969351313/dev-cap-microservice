using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using netcore_auth_svc.Data;
using Newtonsoft.Json.Serialization;
using Infrastructure.ServiceDiscovery;
using OpenTracing;
using OpenTracing.Util;
using System.Reflection;
using Jaeger.Samplers;
using Jaeger;

namespace netcore_auth_svc
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
            ConfigureConsul(services);
            services.AddSingleton<ITracer>(serviceProvider =>  
                {  
                    string serviceName = Assembly.GetEntryAssembly().GetName().Name;  
                
                    ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();  
                
                    ISampler sampler = new ConstSampler(sample: true);  
                
                    ITracer tracer = new Tracer.Builder(serviceName)  
                        .WithLoggerFactory(loggerFactory)  
                        .WithSampler(sampler)  
                        .Build();  
                
                    GlobalTracer.Register(tracer);  
                
                    return tracer;  
                });  
  
            // services.AddOpenTracing(); 
            
            // services.AddMvc()
            services.AddMvcCore()
                .AddAuthorization() 
                .AddJsonFormatters(options =>
                    options.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var connString = Configuration["ConnectionStrings:Default"];
            services.AddDbContext<ApplicationDBContext>(o => o.UseSqlServer(connString));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDBContext>();
           
            services.Configure<IdentityOptions>(options => {
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = Convert.ToInt32(Configuration["User:MaxFailedAccessAttempts"]);
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(Configuration["User:LockoutTime"]));
                options.SignIn.RequireConfirmedEmail = Convert.ToBoolean(Configuration["User:RequireConfirmedEmail"]);
            });

            services.ConfigureApplicationCookie(option=> {
                option.LoginPath = "/Identity/Signin";
                option.AccessDeniedPath = "/Identity/AccessDenied";
                option.ExpireTimeSpan = TimeSpan.FromHours(10);
            });

            services.AddAuthorization(option=> {
                option.AddPolicy("MemberDep", p=> {
                    p.RequireClaim("Department", "Tech", "Full Stack").RequireRole("User");
                });

                option.AddPolicy("AdminDep", p => {
                    p.RequireClaim("Department", "Tech", "hr").RequireRole("Admin");
                });
            });
        }

        
        public void ConfigureConsul(IServiceCollection services)
        {
            var serviceConfig = Configuration.GetServiceConfig();
            services.RegisterConsulServices(serviceConfig);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
