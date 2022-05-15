using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smartstore.Core.Identity;

namespace Smartstore.Moving.Domain
{
    internal class VideoCommentMap : IEntityTypeConfiguration<VideoComment>
    {
        public void Configure(EntityTypeBuilder<VideoComment> builder)
        {
            builder.HasOne(c => c.VideoItem)
                .WithMany(c => c.VideoComments)          // INFO: Important! Must be set in this case else CustomerContent retrieval of type NewsComment will fail.
                .HasForeignKey(c => c.VideoItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    /// <summary>
    /// Represents a news comment.
    /// </summary>
    [Table("NewsComment")] // Enables EF TPT inheritance
    public partial class VideoComment : CustomerContent
    {
        public VideoComment()
        {
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private member.", Justification = "Required for EF lazy loading")]
        private VideoComment(ILazyLoader lazyLoader)
            : base(lazyLoader)
        {
        }

        /// <summary>
        /// Gets or sets the comment title.
        /// </summary>
        [StringLength(450)]
        public string CommentTitle { get; set; }

        /// <summary>
        /// Gets or sets the comment text.
        /// </summary>
        [MaxLength]
        public string CommentText { get; set; }

        /// <summary>
        /// Gets or sets the news item identifier.
        /// </summary>
        public int VideoItemId { get; set; }

        //private VideoItem _videoItem;
        ///// <summary>
        ///// Gets or sets the news item.
        ///// </summary>
        //public VideoItem VideoItem
        //{
        //    get => _videoItem ?? LazyLoader.Load(this, ref _videoItem);
        //    set => _videoItem = value;
        //}
        [ForeignKey("VideoItemId")]
        public virtual VideoItem VideoItem { get; set; }
    }
}
