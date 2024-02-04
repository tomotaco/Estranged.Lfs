using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Estranged.Lfs.Api;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Estranged.Lfs.Adapter.S3;
using Estranged.Lfs.Adapter.DynamoDB;
using Estranged.Lfs.Data;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Estranged.Lfs.Hosting.AspNet
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration credentials = new ConfigurationBuilder().AddJsonFile("credentials.json").Build();

            services.AddLogging(x =>
            {
                x.AddConsole();
                x.AddDebug();
            });

            var options = credentials.GetAWSOptions();
            services.AddDefaultAWSOptions(options);

            services.AddSingleton<IAmazonS3, AmazonS3Client>(); 
            services.AddLfsS3Adapter(new S3BlobAdapterConfig{Bucket = "estranged-lfs-test"}, new AmazonS3Client(options.Credentials));
            services.AddSingleton<IAuthenticator>(x => new DictionaryAuthenticator(new Dictionary<string, string> { { "usernametest", "passwordtest" } }));

            var dynamoDBClient = options.CreateServiceClient<IAmazonDynamoDB>();
            services.AddSingleton<IAmazonDynamoDB>(provider => 
            {
                return dynamoDBClient;
            });
            
            services.AddLfsDynamoDBAdapter(new DynamoDBLockAdapterConfig { TableNamePrefix = "estranged-lfs-test-" }, dynamoDBClient);
            services.AddLfsApi();
            services.AddMvcCore(options =>
            {
                options.EnableEndpointRouting = false;
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "git-lfs Lock API", Version = "v1" });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "git-lfs Lock API");
                c.RoutePrefix = string.Empty;
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}
