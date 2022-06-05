using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Smartstore.Core.Common.Settings;
using Smartstore.Core.Configuration;
using Smartstore.Core.Data;
using Smartstore.Core.Rules.Filters;
using Smartstore.Scheduling;
using Smartstore.Moving.Extensions;
using System.Xml.XPath;
using Smartstore.Core.Content.Media;
using System.Net.Http;

namespace Smartstore.Moving.Tasks
{
    /// <summary>
    /// A task that periodically deletes guest customers.
    /// </summary>
    public partial class JavHDTask : ITask
    {
        private readonly SmartDbContext _db;
        private readonly ILogger _logger;
        private readonly IMediaService _mediaService;

        public JavHDTask(SmartDbContext db, ILogger<JavHDTask> logger, IMediaService mediaService)
        {
            _db = db;
            _logger = logger;
            _mediaService = mediaService;
        }

        public async Task Run(TaskExecutionContext ctx, CancellationToken cancelToken = default)
        {
            var query = _db.Settings.AsNoTracking();
            query = query.ApplySearchFilterFor(x => x.Name, "Admin.Webcrawler.*");
            var settings = await query.ToListAsync();
            var webRecord = settings.FirstOrDefault();
            var url = webRecord.Value;
            AVHandler aVHandler = new AVHandler(url);

            var listItem = aVHandler.CrawlPageList();

            var all_tags = await _db.VideoTags().ToListAsync();
            var dic_Tags = all_tags.GroupBy(g => g.Tag).ToDictionary(x => x.Key, x => x.FirstOrDefault().Id);
            var all_ctgs = all_tags.Where(e => e.IsCategory).GroupBy(g => g.Tag).ToDictionary(x => x.Key, x => x.FirstOrDefault().Id);

            foreach (var item in listItem)
            {
                var video = aVHandler.GetVideo(item, out var ctgs, out var tags);

                var _newCtgs = ctgs.Where(e => !all_ctgs.ContainsKey(e));
                var _newTags = tags.Where(e => !dic_Tags.ContainsKey(e));

                var new_t = _newCtgs.Select(s =>
                {
                    var isPublicCode = s.Trim().StartsWith("[");
                    return new VideoTag
                    {
                        IsCategory = true,
                        Tag = isPublicCode ? s.Trim(new[] { '[', ']' }) : s,
                        IsUseInternal = false,
                        IsPublicCode = isPublicCode, 
                    };
                }).ToList();
                new_t.AddRange(_newTags.Where(e=> !_newCtgs.Contains(e)).Select(s =>
                {
                    var isPublicCode = s.Trim().StartsWith("[");
                    return new VideoTag
                    {
                        IsCategory = false,
                        Tag = isPublicCode ? s.Trim(new[] { '[', ']' }) : s,
                        IsUseInternal = false,
                        IsPublicCode = isPublicCode
                    };
                }));
                new_t = new_t.DistinctBy(d => d.Tag).ToList();
                await _db.VideoTags().AddRangeAsync(new_t);
                

                // renew
                all_tags.AddRange(new_t);
                dic_Tags = all_tags.GroupBy(g => g.Tag).ToDictionary(x => x.Key, x => x.FirstOrDefault().Id);
                all_ctgs = all_tags.Where(e => e.IsCategory).GroupBy(g => g.Tag).ToDictionary(x => x.Key, x => x.FirstOrDefault().Id);

                var imgTHubnail = await SaveToMediaFile(item.Thumbnail, "avvideo");

                video.PreviewMediaFileId = imgTHubnail.Id; 
                await _db.VideoItem().AddAsync(video);

                await _db.SaveChangesAsync();

                var video_tags = all_tags.Where(e => tags.Contains(e.Tag) || ctgs.Contains(e.Tag)).ToList();

                video.VideoItem_VideoTag_Mappings.AddRange(video_tags.Select(s => new VideoItem_VideoTag_Mapping
                {
                    VideoTagId = s.Id,
                    VideoItemId = video.Id
                }));
                await _db.SaveChangesAsync();
            }
            
        }
        public async Task<MediaFileInfo> SaveToMediaFile(string url, string folder)
        {
            var respon = await new HttpClient().GetAsync(url);
            var imgName = new Uri(url).Segments.LastOrDefault();

            var path = _mediaService.CombinePaths(folder, "catalog");
            if (!_mediaService.FolderExists(path))
            {
                await _mediaService.CreateFolderAsync(path);
            } 
            var filePath = _mediaService.CombinePaths(path, imgName);
            var mediaFile = await _mediaService.SaveFileAsync(filePath, await respon.Content.ReadAsStreamAsync(), false, DuplicateFileHandling.Rename);

            return mediaFile;
        }
    }

}

