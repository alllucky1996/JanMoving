using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smartstore.ComponentModel;
using Smartstore.Core.Data;
using Smartstore.Core.Identity;
using Smartstore.Core.Localization;
using Smartstore.Core.Rules.Filters;
using Smartstore.Core.Security;
using Smartstore.Core.Seo;
using Smartstore.Core.Stores;
using Smartstore.Data;
using Smartstore.Moving.Models;
using Smartstore.Web.Controllers;
using Smartstore.Web.Modelling.Settings;
using Smartstore.Web.Models;
using Smartstore.Web.Models.DataGrid;

namespace Smartstore.Moving.Controllers
{
    [Route("[area]/video/{action=index}/{id?}")]
    public class VideoAdminController : AdminController
    {
        private readonly SmartDbContext _db;
        private readonly IUrlService _urlService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICustomerService _customerService;

        public VideoAdminController(
            SmartDbContext db,
            IUrlService urlService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IStoreMappingService storeMappingService,
            ICustomerService customerService)
        {
            _db = db;
            _urlService = urlService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _storeMappingService = storeMappingService;
            _customerService = customerService;
        }

        #region Configure settings

        [Permission(MovingPermissions.Read)]
        [LoadSetting]
        public IActionResult Settings(MovingSettings settings, int storeId)
        {
            var model = MiniMapper.Map<MovingSettings, NewsSettingsModel>(settings);

            AddLocales(model.Locales, (locale, languageId) =>
            {
                locale.MetaTitle = settings.GetLocalizedSetting(x => x.MetaTitle, languageId, storeId, false, false);
                locale.MetaDescription = settings.GetLocalizedSetting(x => x.MetaDescription, languageId, storeId, false, false);
                locale.MetaKeywords = settings.GetLocalizedSetting(x => x.MetaKeywords, languageId, storeId, false, false);
            });

            return View(model);
        }

        [Permission(MovingPermissions.Update)]
        [HttpPost, SaveSetting]
        public async Task<IActionResult> Settings(NewsSettingsModel model, MovingSettings settings, int storeId)
        {
            if (!ModelState.IsValid)
            {
                return Settings(settings, storeId);
            }

            ModelState.Clear();
            MiniMapper.Map(model, settings);

            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.ApplyLocalizedSettingAsync(settings, x => x.MetaTitle, localized.MetaTitle, localized.LanguageId, storeId);
                await _localizedEntityService.ApplyLocalizedSettingAsync(settings, x => x.MetaDescription, localized.MetaDescription, localized.LanguageId, storeId);
                await _localizedEntityService.ApplyLocalizedSettingAsync(settings, x => x.MetaKeywords, localized.MetaKeywords, localized.LanguageId, storeId);
            }

            return RedirectToAction(nameof(Settings));
        }

        #endregion

        #region Utilities

        private async Task UpdateLocalesAsync(VideoItem newsItem, VideoItemModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.ApplyLocalizedValueAsync(newsItem, x => x.Title, localized.Title, localized.LanguageId);
                await _localizedEntityService.ApplyLocalizedValueAsync(newsItem, x => x.Short, localized.Short, localized.LanguageId);
                await _localizedEntityService.ApplyLocalizedValueAsync(newsItem, x => x.Full, localized.Full, localized.LanguageId);
                await _localizedEntityService.ApplyLocalizedValueAsync(newsItem, x => x.MetaKeywords, localized.MetaKeywords, localized.LanguageId);
                await _localizedEntityService.ApplyLocalizedValueAsync(newsItem, x => x.MetaDescription, localized.MetaDescription, localized.LanguageId);
                await _localizedEntityService.ApplyLocalizedValueAsync(newsItem, x => x.MetaTitle, localized.MetaTitle, localized.LanguageId);


                var validateSlugResult = await newsItem.ValidateSlugAsync(localized.SeName, localized.Title, false, localized.LanguageId);
                await _urlService.ApplySlugAsync(validateSlugResult);
                model.SeName = validateSlugResult.Slug;
            }
        }

        private async Task PrepareNewsItemModelAsync(VideoItemModel model, VideoItem newsItem)
        {
            if (newsItem != null)
            {
                model.SelectedStoreIds = await _storeMappingService.GetAuthorizedStoreIdsAsync(newsItem);
            }

            var allLanguages = await _languageService.GetAllLanguagesAsync(true);

            ViewBag.AvailableLanguages = allLanguages
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToList();

            ViewBag.IsSingleLanguageMode = allLanguages.Count <= 1;
            ViewBag.IsSingleStoreMode = Services.StoreContext.IsSingleStoreMode();
        }

        #endregion

        #region videoitems

        public IActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }

        [Permission(MovingPermissions.Read)]
        public async Task<IActionResult> List()
        {
            var model = new VideoListModel
            {
                SearchEndDate = DateTime.UtcNow
            };

            var allLanguages = await _languageService.GetAllLanguagesAsync(true);

            ViewBag.AvailableLanguages = allLanguages
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToList();

            ViewBag.IsSingleLanguageMode = allLanguages.Count <= 1;
            ViewBag.IsSingleStoreMode = Services.StoreContext.IsSingleStoreMode();

            return View(model);
        }

        [HttpPost]
        [Permission(MovingPermissions.Read)]
        public async Task<IActionResult> ListItem(GridCommand command, VideoListModel model)
        {
            var query = _db.VideoItem().Include(x => x.Language).AsNoTracking();

            query = query
                .ApplyStandardFilter(model.SearchStoreId, model.SearchLanguageId, true)
                .Where(x => x.Published == model.SearchIsPublished || model.SearchIsPublished == null);

            if (model.SearchTitle.HasValue())
            {
                query = query.ApplySearchFilterFor(x => x.Title, model.SearchTitle);
            }

            if (model.SearchShort.HasValue())
            {
                query = query.ApplySearchFilterFor(x => x.Short, model.SearchShort);
            }

            if (model.SearchFull.HasValue())
            {
                query = query.ApplySearchFilterFor(x => x.Full, model.SearchFull);
            }

            var newsItems = await query
                .ApplyGridCommand(command)
                .ToPagedList(command)
                .LoadAsync();

            var mapper = MapperFactory.GetMapper<VideoItem, VideoItemModel>();
            var newsItemModels = await newsItems
                .SelectAsync(async x =>
                {
                    var m = await mapper.MapAsync(x);
                    m.EditUrl = Url.Action(nameof(Edit), new { id = m.Id });
                    m.CreatedOn = Services.DateTimeHelper.ConvertToUserTime(x.CreatedOnUtc).ToIso8601String();
                    return m;
                })
                .AsyncToList();

            var gridModel = new GridModel<VideoItemModel>
            {
                Rows = newsItemModels,
                Total = await newsItems.GetTotalCountAsync()
            };

            return Json(gridModel);
        }

        [HttpPost]
        [Permission(MovingPermissions.Update)]
        public async Task<IActionResult> ItemUpdate(VideoItemModel model)
        {
            var success = false;
            var newsItem = await _db.VideoItem().FindByIdAsync(model.Id);

            if (newsItem != null)
            {
                await MapperFactory.MapAsync(model, newsItem);
                await _db.SaveChangesAsync();
                success = true;
            }

            return Json(new { success });
        }

        [Permission(MovingPermissions.Create)]
        public async Task<IActionResult> Create()
        {
            var model = new VideoItemModel
            {
                Published = true,
                AllowComments = true,
                CreatedOnUtc = DateTime.UtcNow
            };

            await PrepareNewsItemModelAsync(model, null);
            AddLocales(model.Locales);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateAntiForgeryToken]
        [Permission(MovingPermissions.Create)]
        public async Task<IActionResult> Create(VideoItemModel model, bool continueEditing, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                var newsItem = await MapperFactory.MapAsync<VideoItemModel, VideoItem>(model);

                _db.VideoItem().Add(newsItem);
                await _db.SaveChangesAsync();

                var validateSlugResult = await newsItem.ValidateSlugAsync(model.SeName, newsItem.Title, true);
                await _urlService.ApplySlugAsync(validateSlugResult);
                model.SeName = validateSlugResult.Slug;

                await UpdateLocalesAsync(newsItem, model);
                await _storeMappingService.ApplyStoreMappingsAsync(newsItem, model.SelectedStoreIds);
                await _db.SaveChangesAsync();

                await Services.EventPublisher.PublishAsync(new ModelBoundEvent(model, newsItem, form));
                NotifySuccess(T("Admin.ContentManagement.News.VideoItem.Added"));

                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = newsItem.Id })
                    : RedirectToAction(nameof(List));
            }

            await PrepareNewsItemModelAsync(model, null);

            return View(model);
        }

        [Permission(MovingPermissions.Read)]
        public async Task<IActionResult> Edit(int id)
        {
            var newsItem = await _db.VideoItem().FindByIdAsync(id, false);
            if (newsItem == null)
            {
                return NotFound();
            }

            var model = await MapperFactory.MapAsync<VideoItem, VideoItemModel>(newsItem);

            AddLocales(model.Locales, async (locale, languageId) =>
            {
                locale.Title = newsItem.GetLocalized(x => x.Title, languageId, false, false);
                locale.Short = newsItem.GetLocalized(x => x.Short, languageId, false, false);
                locale.Full = newsItem.GetLocalized(x => x.Full, languageId, false, false);
                locale.MetaKeywords = newsItem.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = newsItem.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = newsItem.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = await newsItem.GetActiveSlugAsync(languageId, false, false);
            });

            await PrepareNewsItemModelAsync(model, newsItem);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateAntiForgeryToken]
        [Permission(MovingPermissions.Update)]
        public async Task<IActionResult> Edit(VideoItemModel model, bool continueEditing, IFormCollection form)
        {
            var newsItem = await _db.VideoItem().FindByIdAsync(model.Id);
            if (newsItem == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await MapperFactory.MapAsync(model, newsItem);

                var validateSlugResult = await newsItem.ValidateSlugAsync(model.SeName, newsItem.Title, true);
                await _urlService.ApplySlugAsync(validateSlugResult);
                model.SeName = validateSlugResult.Slug;

                await UpdateLocalesAsync(newsItem, model);
                await _storeMappingService.ApplyStoreMappingsAsync(newsItem, model.SelectedStoreIds);
                await _db.SaveChangesAsync();

                await Services.EventPublisher.PublishAsync(new ModelBoundEvent(model, newsItem, form));
                NotifySuccess(T("Admin.ContentManagement.News.VideoItem.Updated"));

                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = newsItem.Id })
                    : RedirectToAction(nameof(List));
            }

            await PrepareNewsItemModelAsync(model, newsItem);

            return View(model);
        }

        [HttpPost]
        [Permission(MovingPermissions.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            var newsItem = await _db.VideoItem().FindByIdAsync(id, false);
            if (newsItem == null)
            {
                return NotFound();
            }

            _db.VideoItem().Remove(newsItem);
            await _db.SaveChangesAsync();

            NotifySuccess(T("Admin.ContentManagement.News.VideoItem.Deleted"));
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        [Permission(MovingPermissions.Delete)]
        public async Task<IActionResult> ItemDelete(GridSelection selection)
        {
            var success = false;
            var numDeleted = 0;
            var ids = selection.GetEntityIds();

            if (ids.Any())
            {
                var newsItems = await _db.VideoItem().GetManyAsync(ids, true);

                _db.VideoItem().RemoveRange(newsItems);

                numDeleted = await _db.SaveChangesAsync();
                success = true;
            }

            return Json(new { Success = success, Count = numDeleted });
        }

        #endregion

        #region Comments

        [Permission(MovingPermissions.Read)]
        public IActionResult Comments(int? newsItemId)
        {
            ViewBag.NewsItemId = newsItemId;

            return View();
        }

        [HttpPost]
        [Permission(MovingPermissions.Read)]
        public async Task<IActionResult> NewsCommentList(int? newsItemId, GridCommand command)
        {
            var query = _db.CustomerContent
                .AsNoTracking()
                .OfType<VideoComment>();

            if (newsItemId.HasValue)
            {
                query = query.Where(x => x.VideoItemId == newsItemId.Value);
            }

            var comments = await query
                .Include(x => x.VideoItem)
                .Include(x => x.Customer)
                .ThenInclude(x => x.CustomerRoleMappings)
                .ThenInclude(x => x.CustomerRole)
                .OrderByDescending(x => x.CreatedOnUtc)
                .ApplyGridCommand(command)
                .ToPagedList(command)
                .LoadAsync();

            var rows = comments.Select(newsComment => new NewsCommentModel
            {
                Id = newsComment.Id,
                NewsItemId = newsComment.VideoItemId,
                NewsItemTitle = newsComment.VideoItem.GetLocalized(x => x.Title),
                CustomerId = newsComment.CustomerId,
                IpAddress = newsComment.IpAddress,
                CreatedOn = Services.DateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc),
                CommentText = newsComment.CommentText.Truncate(270, "…"),
                CommentTitle = newsComment.CommentTitle,
                CustomerName = newsComment.Customer.GetDisplayName(T),
                EditNewsItemUrl = Url.Action(nameof(Edit), "News", new { id = newsComment.VideoItemId }),
                EditCustomerUrl = Url.Action("Edit", "Customer", new { id = newsComment.CustomerId })
            });

            var gridModel = new GridModel<NewsCommentModel>
            {
                Rows = rows,
                Total = await comments.GetTotalCountAsync()
            };

            return Json(gridModel);
        }

        [HttpPost]
        [Permission(MovingPermissions.EditComment)]
        public async Task<IActionResult> NewsCommentDelete(GridSelection selection)
        {
            var success = false;
            var ids = selection.GetEntityIds();

            if (ids.Any())
            {
                var newsComments = await _db.NewsComments().GetManyAsync(ids, true);
                _db.NewsComments().RemoveRange(newsComments);

                await _db.SaveChangesAsync();
                success = true;
            }

            return Json(new { Success = success });
        }

        #endregion
    }
}
