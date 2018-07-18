using EMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using EMS.Repository;

namespace EMS.Filters
{
	public class AuthenticationFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext context)
		{
			try
			{
				var requestHeader = context.Request.Headers;
				if (requestHeader.Contains("access-token"))
				{
					string token = requestHeader.GetValues("access-token").First();
					ClaimsPrincipal principal = JwtValidation.VerifyToken(token);
					if (principal != null)
					{
						var identity = principal.Identity as ClaimsIdentity;
						if (identity != null && identity.IsAuthenticated)
						{
							/*Valid Token - will passed to controller*/
							var claimMail = identity.FindFirst(ClaimTypes.Email);
							string userEmail = claimMail.Value;
							List<Employee> employeeInstance = EmployeeRepo.GetEmployeeByMailId(userEmail);
							if (employeeInstance != null)
							{

							}
							else
							{
								context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
							}
						}
						else
						{
							context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
						}
					}
				}
				else
				{
					context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
				}
				base.OnActionExecuting(context);
			}
			catch (ArgumentException e)
			{
				context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
			}
			catch (Exception e)
			{
				context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
			}
		}
	}
}