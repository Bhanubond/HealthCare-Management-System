using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Controllers
{
    public abstract class BaseController : Controller
    {
        protected void Success(string message)
        {
            TempData["SuccessMessage"] = message;
        }

        protected void Error(string message)
        {
            TempData["ErrorMessage"] = message;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                context.Result = RedirectToAction("Login", "Account");
                return;
            }

            base.OnActionExecuting(context);
        }

        protected void ConfigureList(string title)
        {
            ViewData["Title"] = title;
        }

        protected void ConfigureForm(string title, string formAction, string submitText, params string[] hiddenProperties)
        {
            ViewData["Title"] = title;
            ViewData["FormAction"] = formAction;
            ViewData["SubmitText"] = submitText;
            ViewData["HiddenProperties"] = hiddenProperties;
        }

        protected void ConfigureDetails(string title)
        {
            ViewData["Title"] = title;
        }

        protected void ConfigureDelete(string title, string formAction = "Delete")
        {
            ViewData["Title"] = title;
            ViewData["FormAction"] = formAction;
        }

        protected void SetLookup(string propertyName, IEnumerable<SelectListItem> items)
        {
            ViewData[propertyName] = items;
        }

        protected static SelectList BuildSelectList<T>(IEnumerable<T> items, string valueField, string textField, object? selectedValue = null)
        {
            return new SelectList(items, valueField, textField, selectedValue);
        }
    }
}
