using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smartstore.Core.Identity;
using Smartstore.Domain;

namespace Smartstore.Moving.Domain
{
    internal class VideoTagsMap : IEntityTypeConfiguration<VideoTag>
    {
        public void Configure(EntityTypeBuilder<VideoTag> builder)
        {

        }
    }

    /// <summary>
    /// Represents a news comment.
    /// </summary>
    [Table("VideoTags")] // Enables EF TPT inheritance
    public partial class VideoTag : BaseEntity
    {
        public VideoTag()
        {
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private member.", Justification = "Required for EF lazy loading")]
        private VideoTag(ILazyLoader lazyLoader)
            : base(lazyLoader)
        {
        }

        /// <summary>
        /// Gets or sets the comment title.
        /// </summary>
        [StringLength(200)]
        public string Tag { get; set; }

        public bool IsPublicCode { get; set; }

        private ICollection<VideoItem> _videoItems;
        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        [JsonIgnore]
        public ICollection<VideoItem> VideoItems
        {
            get => _videoItems ?? LazyLoader.Load(this, ref _videoItems) ?? (_videoItems ??= new HashSet<VideoItem>());
            protected set => _videoItems = value;
        }
    }
}
