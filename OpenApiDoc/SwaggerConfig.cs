using ArmsFW.Web.Api;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArmsFW.Web.Api
{
    public class ApiHostSetting
    {
        public List<OpenApiInfoDoc> OpenApiInfoDocs { get; set; }
        public string OpenApiContact_Name { get; set; }
        public string OpenApiContact_Url { get; set; }
        public string OpenApiContact_UrlDoc { get;  set; }
        public string Versao { get; set; }
        public string UrlIcone { get; set; }
        public string UrlDoc { get; set; }
    }


    public class OpenApiInfoDoc 
    {
        public string Title { get; set; }

        public string Description { get; set; }
    }
}


namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Instala o Swagger na api
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Registra o Swagger com todas as informações da API
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiDoc(this IServiceCollection services, Configuration.IConfiguration configuration)
        {

            ApiHostSetting _host = configuration.GetSection("ApiHostSettings").Get<ApiHostSetting>();
            
            services.AddSwaggerGen(c =>
            {
                c.IgnoreObsoleteActions();
                c.OrderActionsBy(x => x.GroupName);

                //Habilita a opção de Autenticacao com Token nas requisicoes de testes do Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Informe um Token Jwt para autorizar as requisição.\r\n\r\nPara obter um novo token, utilize a rota '/api/autenticacao/login'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http, //Com o tipo HTTP, ao realizar a requisicao será automaticamente adicionado o Schema "Bearer" antes do token
                    Scheme = "Bearer", BearerFormat= "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

                var contato = new OpenApiContact
                {
                    Name = _host.OpenApiContact_Name,
                };
                var doc = new OpenApiLicense
                {
                    Name = "Documentação",
                };


                if (!string.IsNullOrEmpty(_host.OpenApiContact_Url)) contato.Url = new Uri(_host.OpenApiContact_Url);
                if (!string.IsNullOrEmpty(_host.OpenApiContact_UrlDoc)) doc.Url = new Uri(_host.OpenApiContact_UrlDoc);


                c.SwaggerDoc(ApiInfo.Versao, new OpenApiInfo { Version = _host.Versao, Title = "Raiz", Description = "APIs sem categoria", Contact = contato, License = doc });

                var _rotasOpenApiInfo = _host.OpenApiInfoDocs;

                if (_rotasOpenApiInfo!=null)
                {
                    _rotasOpenApiInfo.ForEach(doc_info => {

                        c.SwaggerDoc(doc_info.Title, new OpenApiInfo { Version = _host.Versao, Title = doc_info.Title, Description = doc_info.Description, Contact = contato, License = doc });
                    });
                }

                //Paginas do Swagger
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                string arquivoXML = $"{Aplicacao.Executavel.Replace(".dll", ".xml")}";
                if (File.Exists(arquivoXML)) c.IncludeXmlComments(arquivoXML);
            });

            return services;
        }


        /// <summary>
        /// Registra as funcionalidades do Swagger no Pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiDoc(this IApplicationBuilder app, Configuration.IConfiguration configuration)
        {
            ApiHostSetting _host = configuration.GetSection("ApiHostSettings").Get<ApiHostSetting>();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1);

                //Prefixo da rota base da documenacao. Vazio = url raiz
                c.RoutePrefix = _host.UrlDoc;
                c.DocumentTitle = $"Documentação - Api RESTFull - {ApiInfo.Versao}";

                //Define um icone personalizado no favicon
                c.HeadContent = $"<link rel='short cut icon' href='{_host.UrlIcone}' type='image/x-icon' />";

                //Url para as paginas que serão exibidas na combo
                c.SwaggerEndpoint($"/swagger/{ApiInfo.Versao}/swagger.json", "Raiz");

                var _rotasOpenApiInfo = _host.OpenApiInfoDocs;

                if (_rotasOpenApiInfo != null)
                {
                    _rotasOpenApiInfo.ForEach(doc => {
                        c.SwaggerEndpoint($"/swagger/{doc.Title}/swagger.json", doc.Title);
                    });
                }
                //Acresenta CSS na pagina padrao do Swagger
                c.InjectStylesheet("/swagger-ui/custom.css");
                c.InjectJavascript("/swagger-ui/custom.js");
            });

            return app;
        }
    }
}