using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupportLiveStream.Api;
using SupportLiveStream.Data;
using SupportLiveStream.Service;
using SupportLiveStream.Web.AppSettings;
using SupportLiveStream.Web.Middlewares;
using System;

namespace SupportLiveStream.Web
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

            services.AddCors(option =>
            {
                option.AddPolicy(name: "MyPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<JwtConfig>(Configuration.GetSection("jwtConfig"));

            services.AddSingleton<IMongoContext, MongoContext>();

            services.AddSingleton<IFacebookApi, FacebookApi>();
            services.AddSingleton<ISentimentNLPApi, SentimentNLPApi>();

            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IPageRepository, PageRepository>();
            services.AddSingleton<Func<string, IVideoDetailRepository>>(provider => new Func<string, IVideoDetailRepository>((videoId) =>
            {
                return new VideoDetailRepository(provider.GetService<IMongoContext>(), videoId);
            }));

            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<ICommentService, CommentService>();
            services.AddSingleton<IFacebookService, FacebookService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<IPipelineService, PipelineService>();
            services.AddSingleton<Func<string, IVideoDetailService>>(provider => new Func<string, IVideoDetailService>((videoId) =>
            {
                return new VideoDetailServcie(provider.GetService<Func<string, IVideoDetailRepository>>(), videoId);
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("MyPolicy");

            app.UseMiddleware<JwtMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
