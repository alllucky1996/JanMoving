﻿using Smartstore.Collections;

namespace Smartstore.Moving.Models.Public
{
    public partial class NewsItemListModel : ModelBase
    {
        public int WorkingLanguageId { get; set; }
        public bool RenderHeading { get; set; }
        public string NewsHeading { get; set; }
        public bool RssToLinkButton { get; set; }
        public bool DisableCommentCount { get; set; }
        public NewsPagingFilteringModel PagingFilteringContext { get; set; } = new();
        public List<PublicNewsItemModel> NewsItems { get; set; } = new();
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string StoreName { get; set; }
    }

    public partial class NewsPagingFilteringModel : PagedListBase
    {
    }
}
