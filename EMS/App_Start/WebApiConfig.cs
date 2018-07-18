using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using System.Web.Http.Cors;


namespace EMS
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
			config.MapHttpAttributeRoutes();

			var json = config.Formatters.JsonFormatter;
			json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
			json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			json.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
			config.Formatters.Remove(config.Formatters.XmlFormatter);

			//config.Filters.Add(new AuthenticationFilter());
		}
	}
}
