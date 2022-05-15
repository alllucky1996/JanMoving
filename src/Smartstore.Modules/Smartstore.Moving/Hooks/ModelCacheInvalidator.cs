using Smartstore.Caching;
using Smartstore.Core.Configuration;
using Smartstore.Data.Hooks;
using Smartstore.Utilities;

namespace Smartstore.Moving.Hooks
{
    internal partial class ModelCacheInvalidator : IDbSaveHook
    {
        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : store ID
        /// {2} : news count setting.
        /// {3} : whether to include hidden videos.
        /// </remarks>
        public const string HOMEPAGE_NEWSMODEL_KEY = "pres:videos:homepage-{0}-{1}-{2}-{3}";
        public const string NEWS_PATTERN_KEY = "pres:videos:*";

        private readonly static string MainPageNewsCountName = TypeHelper.NameOf<MovingSettings>(x => x.MainPageNewsCount, true);

        private readonly ICacheManager _cache;

        public ModelCacheInvalidator(ICacheManager cache)
        {
            _cache = cache;
        }

        public Task<HookResult> OnBeforeSaveAsync(IHookedEntity entry, CancellationToken cancelToken)
            => Task.FromResult(HookResult.Void);

        public async Task<HookResult> OnAfterSaveAsync(IHookedEntity entry, CancellationToken cancelToken)
        {
            var entity = entry.Entity;
            var result = HookResult.Ok;

            if (entity is VideoItem)
            {
                await _cache.RemoveByPatternAsync(NEWS_PATTERN_KEY);
            }
            else if (entity is Setting)
            {
                var setting = entity as Setting;
                
                if (setting.Name == MainPageNewsCountName)
                {
                    await _cache.RemoveByPatternAsync(NEWS_PATTERN_KEY); // depends on NewsSettings.MainPageNewsCount
                }
            }
            else
            {
                // Register as void hook for all other entity type/state combis
                result = HookResult.Void;
            }

            return result;
        }

        public Task OnBeforeSaveCompletedAsync(IEnumerable<IHookedEntity> entries, CancellationToken cancelToken)
            => Task.CompletedTask;

        public Task OnAfterSaveCompletedAsync(IEnumerable<IHookedEntity> entries, CancellationToken cancelToken)
            => Task.CompletedTask;
    }
}
