using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smartstore.Core.Identity;
using Smartstore.Domain;

namespace Smartstore.Moving.Domain
{
    internal class VideoItemVideoTag_Map : IEntityTypeConfiguration<VideoItem_VideoTag_Mapping>
    {
        public void Configure(EntityTypeBuilder<VideoItem_VideoTag_Mapping> builder)
        {
            ////builder.HasOne(c => c.VideoItem)
            ////    .WithMany()
            ////    .HasForeignKey(c => c.VideoItemId)
            ////    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(c => c.VideoItem)
            //    //.WithMany(c => c.VideoItem_VideoTag_Mappings)
            //    .WithMany()
            //    .HasForeignKey(c => c.VideoItemId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(c => c.VideoTag)
            //    //.WithMany(c => c.VideoItem_VideoTag_Mappings)
            //    .WithMany()
            //    .HasForeignKey(c => c.VideoTagId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
    /// <summary>
    /// Represents a news comment.
    /// </summary>
    [Table("VideoItem_VideoTag_Mapping")] // Enables EF TPT inheritance
    public partial class VideoItem_VideoTag_Mapping : BaseEntity
    {
       
        public int VideoItemId { get; set; }
        public int VideoTagId { get; set; }

        [ForeignKey("VideoItemId")]
        public virtual VideoItem VideoItem { get; set; }
        [ForeignKey("VideoTagId")]
        public virtual VideoTag VideoTag { get; set; }

        //private VideoItem _videoItem;
        ///// <summary>
        ///// Gets or sets the media file.
        ///// </summary>
        //public VideoItem VideoItem
        //{
        //    get => _videoItem ?? LazyLoader?.Load(this, ref _videoItem);
        //    set => _videoItem = value;
        //}
        //private VideoTag _videoTag;
        ///// <summary>
        ///// Gets or sets the media file.
        ///// </summary>
        //public VideoTag VideoTag
        //{
        //    get => _videoTag ?? LazyLoader?.Load(this, ref _videoTag);
        //    set => _videoTag = value;
        //}
    }
}
