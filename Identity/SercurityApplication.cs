using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace app.web.Security
{
	public static class SercurityApplication
	{
		public static async Task<UserToken> BuildToken(ClaimsPrincipal user)
		{
			UserToken userToken = new UserToken();
			try
			{
				List<Claim> list = new List<Claim>
				{
					new Claim("unique_name", user.Identity.Name),
					new Claim("jti", Guid.NewGuid().ToString())
				};
				list.AddRange(user.Claims);
				SigningCredentials signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CD9190C7495A4D1F83CF95E198BF2515")), "HS256");
				DateTime dateTime = DateTime.UtcNow.AddHours(1.0);
				DateTime? expires = dateTime;
				SigningCredentials signingCredentials2 = signingCredentials;
				JwtSecurityToken token = new JwtSecurityToken(null, null, list, null, expires, signingCredentials2);
				userToken = new UserToken
				{
					Token = new JwtSecurityTokenHandler().WriteToken(token),
					Expiration = dateTime,
					TokenID = Guid.NewGuid().ToString(),
					IsValid = true
				};
				userToken.Message = "Token com ID " + userToken.TokenID + " gerado com sucesso.";
			}
			catch (Exception ex)
			{
				userToken.TokenID = "";
				userToken.Message = "Exception em BuildToken() - Ocorreu um erro inesperado ao gerar o Token. Detalhe " + ex.Message;
			}
			return await Task.FromResult(userToken);
		}
	}
}
