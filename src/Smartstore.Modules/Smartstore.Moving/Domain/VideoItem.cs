﻿using System.ComponentModel.DataAnnotations.Schema;
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
    internal class VideoItemMap : IEntityTypeConfiguration<VideoItem>
    {
        public void Configure(EntityTypeBuilder<VideoItem> builder)
        {
            builder.HasOne(c => c.Language)
                .WithMany()
                .HasForeignKey(c => c.LanguageId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.MediaFile)
                .WithMany()
                .HasForeignKey(c => c.MediaFileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.PreviewMediaFile)
                .WithMany()
                .HasForeignKey(c => c.PreviewMediaFileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(p => p.AllowComments).HasDefaultValue(false);

            //builder
            //   .HasMany(c => c.VideoTags)
            //   .WithMany(c => c.VideoItems)
            //   .UsingEntity<Dictionary<string, object>>(
            //       "VideoItem_VideoTag_Mapping",
            //       c => c
            //           .HasOne<VideoTag>()
            //           .WithMany()
            //           .HasForeignKey("VideoTag_Id")
            //           .HasConstraintName("FK_dbo.VideoItem_VideoTag_Mapping_dbo.VideoTag_VideoTag_Id")
            //           .OnDelete(DeleteBehavior.Cascade),
            //       c => c
            //           .HasOne<VideoItem>()
            //           .WithMany()
            //           .HasForeignKey("VideoItem_Id")
            //           .HasConstraintName("FK_dbo.VideoItem_VideoTag_Mapping_dbo.VideoItem_VideoItem_Id")
            //           .OnDelete(DeleteBehavior.Cascade),
            //       c =>
            //       {
            //           c.HasIndex("VideoItem_Id");
            //           c.HasKey("VideoItem_Id", "VideoTag_Id");
            //       });
        }
    }

    /// <summary>
    /// Represents a news item.
    /// </summary>
    [Table(nameof(VideoItem))]
    [Index(nameof(Title), Name = "IX_Title")]
    public partial class VideoItem : BaseEntity, ISlugSupported, IStoreRestricted, ILocalizedEntity, IAuditable
    {
        #region static

        private static readonly List<string> _visibilityAffectingProps = new()
        {
            nameof(Published),
            nameof(StartDateUtc),
            nameof(EndDateUtc),
            nameof(LimitedToStores)
        };

        public static IReadOnlyCollection<string> GetVisibilityAffectingPropertyNames()
        {
            return _visibilityAffectingProps;
        }

        #endregion

        [Required, StringLength(255)]
        public string Url { get; set; }
        [Required, StringLength(255)]
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the news title.
        /// </summary>
        [Required, StringLength(450)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the short text.
        /// </summary>
        [Required, StringLength(4000)]
        public string Short { get; set; }

        /// <summary>
        /// Gets or sets the full text.
        /// </summary>
        [Required, MaxLength]
        public string Full { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the news item is published.
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the media file identifier.
        /// </summary>
        public int? MediaFileId { get; set; }

        private MediaFile _mediaFile;
        /// <summary>
        /// Gets or sets the media file.
        /// </summary>
        public MediaFile MediaFile
        {
            get => _mediaFile ?? LazyLoader.Load(this, ref _mediaFile);
            set => _mediaFile = value;
        }

        /// <summary>
        /// Gets or sets the preview media file identifier.
        /// </summary>
        public int? PreviewMediaFileId { get; set; }

        private MediaFile _previewMediaFile;
        /// <summary>
        /// Gets or sets the preview media file.
        /// </summary>
        public MediaFile PreviewMediaFile
        {
            get => _previewMediaFile ?? LazyLoader.Load(this, ref _previewMediaFile);
            set => _previewMediaFile = value;
        }

        /// <summary>
        /// Gets or sets the news item start date and time.
        /// </summary>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the news item end date and time.
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the news post comments are allowed.
        /// </summary>
        public bool AllowComments { get; set; }

        /// <summary>
        /// Gets or sets the total number of approved comments.
        /// <remarks>The same as if we run newsItem.NewsComments.Where(n => n.IsApproved).Count().
        /// We use this property for performance optimization (no SQL command executed).
        /// </remarks>
        /// </summary>
        public int ApprovedCommentCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of not approved comments.
        /// <remarks>The same as if we run newsItem.NewsComments.Where(n => !n.IsApproved).Count().
        /// We use this property for performance optimization (no SQL command executed).
        /// </remarks>
        /// </summary>
        public int NotApprovedCommentCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores.
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation.
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords.
        /// </summary>
        [StringLength(400)]
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description.
        /// </summary>
        [StringLength(4000)]
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title.
        /// </summary>
        [StringLength(400)]
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets a language identifier for which the news item should be displayed.
        /// </summary>
        public int? LanguageId { get; set; }

        private Language _language;
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public Language Language
        {
            get => _language ?? LazyLoader.Load(this, ref _language);
            set => _language = value;
        }

       

        /// <inheritdoc/>
        public string GetDisplayName()
        {
            return Title;
        }

        /// <inheritdoc/>
        public string GetDisplayNameMemberName()
        {
            return nameof(Title);
        }

        public virtual ICollection<VideoItem_VideoTag_Mapping> VideoItem_VideoTag_Mappings { get; set; } = new List<VideoItem_VideoTag_Mapping>();
        public virtual ICollection<VideoComment> VideoComments { get; set; } = new List<VideoComment>();

        //private ICollection<VideoComment> _videoComments;
        ///// <summary>
        ///// Gets or sets the news comments.
        ///// </summary>
        //public ICollection<VideoComment> VideoComments
        //{
        //    get => _videoComments ?? LazyLoader.Load(this, ref _videoComments) ?? (_videoComments ??= new HashSet<VideoComment>());
        //    set => _videoComments = value;
        //}

        //private ICollection<VideoItem_VideoTag_Mapping> _videoTags;
        ///// <summary>
        ///// Gets or sets the product tags.
        ///// </summary>
        //public ICollection<VideoItem_VideoTag_Mapping> VideoItem_VideoTag_Mappings
        //{
        //    get => LazyLoader?.Load(this, ref _videoTags) ?? (_videoTags ??= new HashSet<VideoItem_VideoTag_Mapping>());
        //    protected set => _videoTags = value;
        //}
        public DateTime UpdatedOnUtc { get; set; }
    }


public enum HttpType
{
    Get,
    Post,
    Delete
}
}
