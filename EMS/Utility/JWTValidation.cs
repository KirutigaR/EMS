using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EMS.Utility
{
	public class JwtValidation
	{
		/// <summary>
		/// create a JWT token using given claim identities(Email ID)
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public static string CreateToken(string email)
		{
			var symmetricKey = Convert.FromBase64String("j2ffC6A0jgVPIORNQuAqVhUI3PLhp4Vlo3XDrXWO");
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] {
								new Claim(ClaimTypes.Email, email),
							}),
				Expires = DateTime.Now.AddMinutes(120),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
			return token;
		}

		/// <summary>
		/// Verify JWT based on created and expiry time
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public static ClaimsPrincipal VerifyToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
			if (jwtToken != null)
			{
				var symmetricKey = Convert.FromBase64String("j2ffC6A0jgVPIORNQuAqVhUI3PLhp4Vlo3XDrXWO");
				var validationParameters = new TokenValidationParameters()
				{
					RequireExpirationTime = true,
					ValidateIssuer = false,
					ValidateAudience = false,
					IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
					ClockSkew = TimeSpan.Zero
				};
				ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
				return claimsPrincipal;
			}
			else
			{
				return null;
			}
		}
	}
}
