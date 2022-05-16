using System.Data;
using FluentMigrator;
using Smartstore.Core.Content.Media;
using Smartstore.Core.Data.Migrations;
using Smartstore.Core.Identity;
using Smartstore.Core.Localization;
using Smartstore.Domain;
//FluentMigrator.Generator

namespace Smartstore.Moving.Migrations
{
    [MigrationVersion("2022-05-16 15:30:00", "Videos: Initial")]
    internal class Initial : Migration
    {
        public override void Up()
        {
            const string id = nameof(BaseEntity.Id);

            #region VideoItems
            {

                if (!Schema.Table(nameof(VideoItem)).Exists())
                {

                    Create.Table(nameof(VideoItem))
                        .WithIdColumn()
                        .WithColumn(nameof(VideoItem.Title)).AsString(450).NotNullable()
                            .Indexed("IX_Title")
                        .WithColumn(nameof(VideoItem.Short)).AsString(4000).NotNullable()
                        .WithColumn(nameof(VideoItem.Full)).AsMaxString().NotNullable()
                        .WithColumn(nameof(VideoItem.Published)).AsBoolean().NotNullable()
                        .WithColumn(nameof(VideoItem.StartDateUtc)).AsDateTime2().Nullable()
                        .WithColumn(nameof(VideoItem.EndDateUtc)).AsDateTime2().Nullable()
                        .WithColumn(nameof(VideoItem.AllowComments)).AsBoolean().NotNullable()
                        .WithColumn(nameof(VideoItem.ApprovedCommentCount)).AsInt32().NotNullable()
                        .WithColumn(nameof(VideoItem.NotApprovedCommentCount)).AsInt32().NotNullable()
                        .WithColumn(nameof(VideoItem.LimitedToStores)).AsBoolean().NotNullable()
                        .WithColumn(nameof(VideoItem.CreatedOnUtc)).AsDateTime2().NotNullable()
                        .WithColumn(nameof(VideoItem.MetaKeywords)).AsString(400).Nullable()
                        .WithColumn(nameof(VideoItem.MetaDescription)).AsString(4000).Nullable()
                        .WithColumn(nameof(VideoItem.MetaTitle)).AsString(400).Nullable()
                        .WithColumn(nameof(VideoItem.MediaFileId)).AsInt32().Nullable()
                            .Indexed("IX_MediaFileId")
                            .ForeignKey(nameof(MediaFile), id).OnDelete(Rule.None)
                        .WithColumn(nameof(VideoItem.PreviewMediaFileId)).AsInt32().Nullable()
                            .Indexed("IX_PreviewMediaFileId")
                            .ForeignKey(nameof(MediaFile), id).OnDelete(Rule.None)
                        .WithColumn(nameof(VideoItem.LanguageId)).AsInt32().Nullable()
                            .Indexed("IX_LanguageId")
                            .ForeignKey(nameof(Language), id).OnDelete(Rule.None);
                }
            }
            #endregion
            #region VideoComment
            {

                if (!Schema.Table(nameof(VideoComment)).Exists())
                {

                    Create.Table(nameof(VideoComment))
                        .WithIdColumn()
                        .WithColumn(nameof(VideoComment.CommentTitle)).AsString(450).Nullable()
                        .WithColumn(nameof(VideoComment.CommentText)).AsMaxString().Nullable()
                        .WithColumn(nameof(VideoComment.VideoItemId)).AsInt32().NotNullable()
                            .Indexed().ForeignKey(nameof(VideoItem), id).OnDelete(Rule.Cascade);

                    Create.ForeignKey()
                      .FromTable(nameof(VideoComment)).ForeignColumn(id)
                      .ToTable(nameof(CustomerContent)).PrimaryColumn(id);
                }
            }
            #endregion
            #region VideoTags

            if (!Schema.Table(nameof(VideoTag)).Exists())
            {

                Create.Table(nameof(VideoTag))
                    .WithIdColumn()
                    .WithColumn(nameof(VideoTag.Tag)).AsString(200).Nullable()
                    .WithColumn(nameof(VideoTag.IsPublicCode)).AsBoolean().NotNullable()
                            .Indexed("IX_IsPublicCode");
            }
            #endregion
            #region VideoTags

            if (!Schema.Table(nameof(VideoMedia)).Exists())
            {

                Create.Table(nameof(VideoMedia))
                    .WithIdColumn()
                    .WithColumn(nameof(VideoMedia.WebTarget)).AsString().Nullable()
                        .Indexed("IX_WebTarget")
                    .WithColumn(nameof(VideoMedia.MediaType)).AsString().Nullable()
                    .WithColumn(nameof(VideoMedia.Alt)).AsString().Nullable()
                    .WithColumn(nameof(VideoMedia.Name)).AsString().Nullable()
                        .Indexed("IX_Video_Name")
                    .WithColumn(nameof(VideoMedia.Size)).AsInt32().NotNullable()
                        .Indexed("IX_Video_Size")
                    .WithColumn(nameof(VideoMedia.PixelSize)).AsInt32().Nullable()
                    .WithColumn(nameof(VideoMedia.Width)).AsInt32().Nullable()
                    .WithColumn(nameof(VideoMedia.Height)).AsInt32().Nullable()
                    .WithColumn(nameof(VideoMedia.CreatedOnUtc)).AsDateTime2().NotNullable()
                    .WithColumn(nameof(VideoMedia.UpdatedOnUtc)).AsDateTime2().NotNullable()
                        .Indexed("IX_Video_UpdatedOnUtc")
                    .WithColumn(nameof(VideoMedia.LimitedToStores)).AsBoolean().NotNullable()
                    .WithColumn(nameof(VideoMedia.Deleted)).AsBoolean().NotNullable()
                        .Indexed("IX_Video_Delete")
                    .WithColumn(nameof(VideoMedia.IsTransient)).AsBoolean().NotNullable();
            }
            #endregion



        }

        public override void Down()
        {
            // INFO: no down initial migration. Leave news schema as it is or ask merchant to delete it.
        }
    }
}
