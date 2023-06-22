using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace FamilyRealtor.Helpers
{
	public static class ViewRenderer
	{
		public static async Task<string> RenderViewToStringAsync(Controller controller, string viewName, object model, bool isPartial)
		{
			IServiceProvider serviceProvider = controller.HttpContext.RequestServices;

			ICompositeViewEngine? viewEngine = serviceProvider.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine
				?? throw new InvalidOperationException();

			ViewEngineResult viewEngineResult = viewEngine.FindView(controller.ControllerContext, viewName, isMainPage: !isPartial);
			if (!viewEngineResult.Success) throw new InvalidOperationException($"Частичное представление '{viewName}' не найдено.");

			using (StringWriter sw = new())
			{
				ViewContext viewContext = new(
					controller.ControllerContext,
					viewEngineResult.View,
					controller.ViewData,
					controller.TempData,
					sw,
					new HtmlHelperOptions()
				);

				viewContext.ViewData.Model = model;
				await viewEngineResult.View.RenderAsync(viewContext);

				return sw.ToString();
			}
		}
	}
}