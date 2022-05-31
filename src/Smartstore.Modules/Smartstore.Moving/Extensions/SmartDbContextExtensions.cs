using Smartstore.Core.Data;

namespace Smartstore.Moving
{
    public static class SmartDbContextExtensions
    {
        public static DbSet<VideoItem> VideoItem(this SmartDbContext db)
            => db.Set<VideoItem>();

        public static DbSet<VideoComment> NewsComments(this SmartDbContext db)
            => db.Set<VideoComment>();
    }
}
