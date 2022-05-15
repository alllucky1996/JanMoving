using Smartstore.Core.Data;
using Smartstore.Data.Hooks;

namespace Smartstore.Moving.Hooks
{
    [Important]
    internal class NewsCommentHook : AsyncDbSaveHook<VideoComment>
    {
        private readonly SmartDbContext _db;

        public NewsCommentHook(SmartDbContext db)
        {
            _db = db;
        }

        protected override Task<HookResult> OnInsertedAsync(VideoComment entity, IHookedEntity entry, CancellationToken cancelToken)
            => Task.FromResult(HookResult.Ok);

        protected override Task<HookResult> OnDeletedAsync(VideoComment entity, IHookedEntity entry, CancellationToken cancelToken)
            => Task.FromResult(HookResult.Ok);

        /// <summary>
        /// Update approved comment count after adding or removing a newscomment.
        /// </summary>
        public override async Task OnAfterSaveCompletedAsync(IEnumerable<IHookedEntity> entries, CancellationToken cancelToken)
        {
            var comments = entries
                .Select(x => x.Entity)
                .OfType<VideoComment>()
                .ToList();

            var itemIds = comments.Select(x => x.VideoItemId).Distinct().ToArray();

            foreach (var itemId in itemIds)
            {
                var newsItem = await _db.NewsItems().FindByIdAsync(itemId);
                if (newsItem != null)
                {
                    var query = _db.NewsComments();

                    newsItem.ApprovedCommentCount = query.Where(x => x.VideoItemId == itemId && x.IsApproved).Count();
                    newsItem.NotApprovedCommentCount = query.Where(x => x.VideoItemId == itemId && !x.IsApproved).Count();
                }
            }

            await _db.SaveChangesAsync(cancelToken);
        }
    }
}
