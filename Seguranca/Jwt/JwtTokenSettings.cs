using ArmsFW.Core;
using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Shared.Settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

/// <summary>
/// Contem objetos para tratativa de security
/// </summary>
namespace ArmsFW.Security
{
    /// <summary>
    /// Implementação para parametros do token Jwt
    /// </summary>
    public class JwtTokenSettings : TokenValidationParameters
    {
        //Default
        public JwtTokenSettings()
        {
            ValidateIssuer = false;
            ValidateAudience = false;
            //ValidateLifetime = true;
            //ValidateIssuerSigningKey = true;
            ClockSkew = TimeSpan.Zero;
        }
        internal string Secret => "JWT-KEY-ARMSFW-998877665544332211-112233445566778899-FINAL-JWT-KEY-ARMSFW";

        public new SecurityKey IssuerSigningKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Secret));

        public DateTime? LoadedDateTime { get; set; }

        public static JwtTokenSettings Load()
        {

            var st = new ConfiguracaoBase<JwtTokenSettings>();

            st.FilePathSettings = $"{Aplicacao.DiretorioStore}\\JwtTokenSettings.json";

            var jsonResult = JSON.LoadFromFile<JwtTokenSettings>(st.FilePathSettings);

            var s = jsonResult.Result;

            s.LoadedDateTime = DateTime.Now;
            return s;
        }

        public override string ToString() => $"{(this.Secret == null ? "Secret : (Empty)" : "Secret : Loaded...")} / Loaded : {LoadedDateTime}";
    }
}


