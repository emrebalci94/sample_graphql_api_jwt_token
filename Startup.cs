using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sample_graphql_api.Graphql;
using sample_graphql_api.Graphql.Types;
using sample_graphql_api.Helpers;
using sample_graphql_api.Extensions;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
namespace sample_graphql_api
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

            services.AddSingleton<MarketingContext>();
            services.AddSingleton<IDependencyResolver>(_ => new FuncDependencyResolver(_.GetRequiredService));
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<MaterialType>();
            services.AddSingleton<BrandType>();

            services.AddSingleton<MarketingQuery>();
            services.AddSingleton<ISchema, MarketingSchema>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // services.AddJwtToken(Configuration); //JWT Token'ı Extension Method olarak eklemek istersek.

            #region JWT Token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           //JWT Tokenın ayarlarını yaptığımız yerdir.
           options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
           {
               //Token alıcısına vereceğimiz özel bilgi. Bu bilgiyle token oluşacak.Böylece bu bilgi içermeyen tokenlarla giriş yapmaya çalışanlar giriş yapamamış olacak.
               ValidAudience = (string)Convert.ChangeType(Configuration["JwtTokenConfig:ValidAudience"], typeof(string)),
               //Token alıcısının izin verdiğimiz kullanıcımı bunu kontrol etmesi için kullanılır. ValidAudience bilgisi benim token oluştururken tekrar geçmeliyim ki o zaman izin versin.
               ValidateAudience = (bool)Convert.ChangeType(Configuration["JwtTokenConfig:ValidateAudience"], typeof(bool)),
               //Token oluştururken vereceğimiz bilgi, Tokenı hangi site için oluşturduğumuzu veriyoruz.
               ValidIssuer = (string)Convert.ChangeType(Configuration["JwtTokenConfig:ValidIssuer"], typeof(string)),
               //Token bilgisindeki ValidIssuer bilgisi bizim serverda oluşturduğumuzla aynı mı.  Hangi site bu tokenı verdi bilgisini burada veriyoruz. Eğer bu bilgilerde farklıysa giriş işlemi başarısız olur.
               ValidateIssuer = (bool)Convert.ChangeType(Configuration["JwtTokenConfig:ValidateIssuer"], typeof(bool)),
               //Aslında bunlar sayesinde vereceğimiz tokena ekstra güvenlik yapısı ekliyoruz diyebiliriz.
               //Token bilgisinin ExpiresDate(Ömrü) olup olmadığına bakmak için kullanıyoruz.
               ValidateLifetime = (bool)Convert.ChangeType(Configuration["JwtTokenConfig:ValidateLifetime"], typeof(bool)),
               //Oluşturulacak olan tokenın güvenlik anahtarı oluşturmak için.
               IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes((string)Convert.ChangeType(Configuration["JwtTokenConfig:IssuerSigningKey"], typeof(string)))),
               //Oluşturulan token hangi standarta uygun olarak oluşturulduğuna bakılsın mı.
               ValidateIssuerSigningKey = (bool)Convert.ChangeType(Configuration["JwtTokenConfig:ValidateIssuerSigningKey"], typeof(bool)),
               //Expires süresinin minimum ne kadar olduğunu ayarlıyoruz. Defaultta 5 dakikadır. Eğer bu değeri vermezsek ve ExpiresDate'i 5 dakikadan az girersek token 5 dakikadan önce düşmeyecek o yüzden bu kısmı 0 olarak ayarlıyoruz.
               ClockSkew = TimeSpan.Zero
           };

           //JWT nin eventlarını yakaldığımız yer, örneğin Token yanlış yada token doğru gibi eventları yakalamak için kullanılıyor.
           options.Events = new JwtBearerEvents
           {
               //Hatalı bir authentication işlemi varsa
               OnAuthenticationFailed = _ =>
          {
              Console.WriteLine($"Exception:{_.Exception.Message}");
              return Task.CompletedTask;
          },
               //Eğer token token doğruysa.
               OnTokenValidated = _ =>
          {
              Console.WriteLine($"Login Success:{ _.Principal.Identity}");
              return Task.CompletedTask;
          },

           };
       });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();//Token için gerekli.

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
