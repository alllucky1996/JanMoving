﻿using Humanizer;
using Smartstore.ComponentModel;
using Smartstore.Core;
using Smartstore.Core.Identity;
using Smartstore.Core.Localization;
using Smartstore.Core.Security;
using Smartstore.Core.Seo;
using Smartstore.Moving.Models.Public;
using Smartstore.Utilities.Html;
using Smartstore.Web.Models.Common;
using Smartstore.Web.Models.Customers;
using Smartstore.Web.Models.Media;

namespace Smartstore.Moving.Models.Mappers
{
    public static partial class NewsMappingExtensions
    {
        public static async Task<PublicNewsItemModel> MapAsync(this VideoItem entity, dynamic parameters = null)
        {
            var model = new PublicNewsItemModel();
            await MapAsync(entity, model, parameters);

            return model;
        }

        public static async Task MapAsync(this VideoItem entity, PublicNewsItemModel model, dynamic parameters = null)
        {
            await MapperFactory.MapAsync(entity, model, parameters);
        }
    }

    public class NewsItemMapper : Mapper<VideoItem, PublicNewsItemModel>
    {
        private readonly ICommonServices _services;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;

        public NewsItemMapper(ICommonServices services, CustomerSettings customerSettings, CaptchaSettings captchaSettings)
        {
            _services = services;
            _customerSettings = customerSettings;
            _captchaSettings = captchaSettings;
        }

        public Localizer T { get; set; } = NullLocalizer.Instance;

        protected override void Map(VideoItem from, PublicNewsItemModel to, dynamic parameters = null)
            => throw new NotImplementedException();

        public override async Task MapAsync(VideoItem from, PublicNewsItemModel to, dynamic parameters = null)
        {
            Guard.NotNull(from, nameof(from));
            Guard.NotNull(to, nameof(to));

            var dtHelper = _services.DateTimeHelper;
            var mapper = MapperFactory.GetMapper<VideoItem, ImageModel>();
            var prepareComments = Convert.ToBoolean(parameters?.PrepareComments as bool?);

            _services.DisplayControl.Announce(from);

            MiniMapper.Map(from, to);

            to.Title = from.GetLocalized(x => x.Title);
            to.Short = from.GetLocalized(x => x.Short);
            to.Full = from.GetLocalized(x => x.Full, true);
            to.MetaTitle = from.GetLocalized(x => x.MetaTitle);
            to.MetaDescription = from.GetLocalized(x => x.MetaDescription);
            to.MetaKeywords = from.GetLocalized(x => x.MetaKeywords);
            to.SeName = await from.GetActiveSlugAsync(ensureTwoPublishedLanguages: false);
            to.CreatedOn = dtHelper.ConvertToUserTime(from.CreatedOnUtc, DateTimeKind.Utc);
            to.CreatedOnUTC = from.CreatedOnUtc;
            to.AddNewComment.DisplayCaptcha = _captchaSettings.CanDisplayCaptcha && _captchaSettings.ShowOnNewsCommentPage;
            to.DisplayAdminLink = _services.Permissions.Authorize(Permissions.System.AccessBackend, _services.WorkContext.CurrentCustomer);

            to.PictureModel = await mapper.MapAsync(from, new { FileId = from.MediaFileId });
            to.PreviewPictureModel = await mapper.MapAsync(from, new { FileId = from.PreviewMediaFileId });
            to.Comments.AllowComments = from.AllowComments;
            to.Comments.NumberOfComments = from.ApprovedCommentCount;
            to.Comments.AllowCustomersToUploadAvatars = _customerSettings.AllowCustomersToUploadAvatars;
            to.MetaProperties = await to.MapMetaPropertiesAsync();

            if (prepareComments)
            {
                var newsComments = from.VideoComments
                    .Where(x => x.IsApproved)
                    .OrderBy(x => x.CreatedOnUtc);

                foreach (var nc in newsComments)
                {
                    var commentModel = new CommentModel(to.Comments)
                    {
                        Id = nc.Id,
                        CustomerId = nc.CustomerId,
                        CustomerName = nc.Customer.FormatUserName(_customerSettings, T, false),
                        CommentTitle = HtmlUtility.StripTags(nc.CommentTitle),
                        CommentText = HtmlUtility.SanitizeHtml(nc.CommentText, HtmlSanitizerOptions.UserCommentSuitable),
                        CreatedOn = dtHelper.ConvertToUserTime(nc.CreatedOnUtc, DateTimeKind.Utc),
                        CreatedOnPretty = dtHelper.ConvertToUserTime(nc.CreatedOnUtc, DateTimeKind.Utc).Humanize(false),
                        AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !nc.Customer.IsGuest(),
                    };

                    commentModel.Avatar = await nc.Customer.MapAsync();

                    to.Comments.Comments.Add(commentModel);
                }
            }
        }
    }
}
