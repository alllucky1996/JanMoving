﻿using Smartstore.Collections;

namespace Smartstore.Forums.Models.Public
{
    public partial class ActiveDiscussionsModel : ModelBase
    {
        public bool IsActiveDiscussionsPage { get; set; }
        public bool ActiveDiscussionsFeedEnabled { get; set; }

        public IPagedList<PublicForumTopicModel> ForumTopics { get; set; }
    }
}
