using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smartstore.Core.Content.Media;
using Smartstore.Core.Localization;
using Smartstore.Core.Seo;
using Smartstore.Core.Stores;
using Smartstore.Domain;

namespace Smartstore.Moving.Domain
{
    // TODO: (mg) (core) find alternative solution for NewsItem.NewsImporterGuid and index IX_News_NewsImporterGuid (required by News-Importer).
    internal class VideoMediaMap : IEntityTypeConfiguration<VideoMedia>
    {
        public void Configure(EntityTypeBuilder<VideoMedia> builder)
        {
            
        }
    }

    /// <summary>
    /// Represents a VideoMedia item.
    /// </summary>
    [Table("VideoMedia")]
    [Index(nameof(WebTarget), Name = "IX_WebTarget")]
    [Index(nameof(Deleted), Name = "IX_Video_Delete")] 
    [Index(nameof(Name), nameof(Deleted), Name = "IX_Video_Name")] 
    [Index(nameof(Size), nameof(Deleted), Name = "IX_Video_Size")] 
    // Exact Core port. UpdatedOnUtc was mistakenly missing in the original implementation.
    [Index(nameof(UpdatedOnUtc), Name = "IX_Video_UpdatedOnUtc")]
    public partial class VideoMedia : BaseEntity, IStoreRestricted, ISoftDeletable, IAuditable
    {

        
        public string WebTarget { get; set; }
        [StringLength(20)]
        public string MediaType { get; set; }
        public string Alt { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the total pixel size of an image (width * height).
        /// </summary>
        public int? PixelSize { get; set; }
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the image height (if file is an image).
        /// </summary>
        public int? Height { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedOnUtc { get; set; }

        /// <inheritdoc/>
        public DateTime UpdatedOnUtc { get; set; }
        public bool LimitedToStores { get; set; }
        public bool Deleted { get; set; }
        public bool IsTransient { get; set; }
        
    }
}
