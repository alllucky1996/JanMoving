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
            //builder.Property(p=>p.IsUseInternal).HasDefaultValue(false);
        }
    }

    /// <summary>
    /// Represents a news comment.
    /// </summary>
    [Table(nameof(VideoTag))] // Enables EF TPT inheritance
    public partial class VideoTag : BaseEntity
    {
       

        /// <summary>
        /// Gets or sets the comment title.
        /// </summary>
        [StringLength(200)]
        public string Tag { get; set; }

        public bool IsPublicCode { get; set; }
        public bool IsUseInternal { get; set; }
        public bool IsCategory { get; set; }

        public virtual ICollection<VideoItem_VideoTag_Mapping> VideoItem_VideoTag_Mapping { get; set; }

        //private ICollection<VideoItem_VideoTag_Mapping> _videoTags;
        ///// <summary>
        ///// Gets or sets the product tags.
        ///// </summary>
        //public ICollection<VideoItem_VideoTag_Mapping> VideoItem_VideoTag_Mappings
        //{
        //    get => LazyLoader?.Load(this, ref _videoTags) ?? (_videoTags ??= new HashSet<VideoItem_VideoTag_Mapping>());
        //    protected set => _videoTags = value;
        //}

        //private ICollection<VideoItem> _videoItems;
        ///// <summary>
        ///// Gets or sets the products.
        ///// </summary>
        //[JsonIgnore]
        //public ICollection<VideoItem> VideoItems
        //{
        //    get => _videoItems ?? LazyLoader.Load(this, ref _videoItems) ?? (_videoItems ??= new HashSet<VideoItem>());
        //    protected set => _videoItems = value;
        //}
    }
}
