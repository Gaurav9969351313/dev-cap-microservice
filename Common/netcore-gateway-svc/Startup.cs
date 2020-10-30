using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using OpenTracing.Util;

namespace netcore_gateway_svc
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<ITracer>(cli =>
                {
                    Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", Configuration["LatencyTrace:JAEGER_SERVICE_NAME"]);
                    Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST",  Configuration["LatencyTrace:JAEGER_AGENT_HOST"]);
                    Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT",  Configuration["LatencyTrace:JAEGER_AGENT_PORT"]);
                    Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE",  Configuration["LatencyTrace:JAEGER_SAMPLER_TYPE"]);

                    var loggerFactory = new LoggerFactory();

                    var config = Jaeger.Configuration.FromEnv(loggerFactory);
                    var tracer = config.GetTracer();

                    if (!GlobalTracer.IsRegistered())
                    {
                        // Allows code that can't use DI to also access the tracer.
                        GlobalTracer.Register(tracer);
                    }

                    return tracer;
                });
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

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
