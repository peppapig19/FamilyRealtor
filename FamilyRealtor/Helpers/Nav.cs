using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyRealtor.Helpers
{
    public static class Nav
    {
		public static string ActiveArea(ViewContext ctx, string area)
		{
			string? routeArea = ctx.RouteData.Values["area"]?.ToString();

			if (area == routeArea) return "active";
			return "";
		}

		public static string Active(ViewContext ctx, string? area = null, string? controller = null, string? action = null)
		{
			string? routeArea = ctx.RouteData.Values["area"]?.ToString();
			string? routeController = ctx.RouteData.Values["controller"]?.ToString();
			string? routeAction = ctx.RouteData.Values["action"]?.ToString();

			if (area == routeArea && controller == routeController && (action == null || action == routeAction)) return "active";
			return "";
		}
	}
}