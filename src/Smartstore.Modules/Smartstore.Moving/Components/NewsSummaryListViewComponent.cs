using Microsoft.AspNetCore.Mvc;
using Smartstore.Moving.Models.Public;
using Smartstore.Web.Components;

namespace Smartstore.Moving.Components
{
    /// <summary>
    /// Component to render news item list via module partial & page builder block.
    /// </summary>
    public class NewsSummaryListViewComponent : SmartViewComponent
    {
        public IViewComponentResult Invoke(NewsItemListModel model)
        {
            return View(model);
        }
    }
}
