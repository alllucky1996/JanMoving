using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Smartstore.Moving.Tasks
{
    public sealed class AVExcception : Exception
    {
        public AVExcception(string msg) : base(msg)
        {
        }
        public AVExcception(Exception inner) : base("Đã có lỗi sảy ra. xem chi tiết trong exceptions", inner)
        {
        }
    }
    /// <summary>
    /// Base javhd.onl, javhd.pro
    /// </summary>
    public class AVHandler
    {
        protected Uri Uri { get; set; }
        protected HtmlDocument Document { get; set; }
        protected ICollection<AVExcception> Excceptions { get; set; }
        public AVHandler(string url)
        {
            Uri = new Uri(url);
            Document = new HtmlWeb().Load(url);
        }
        public List<Item> CrawlPageList()
        {
            // movie-last-movie

            //// ex: https://javhd.onl/search/china/page/1
            //var mainContentId = "main-content";
            //var listSelector = "movie-last-movie"; // ==> ul

            //var allTag_a = Document.DocumentNode.SelectNodes("//a[@class='movie-item']");
            var allTag_a = Document.DocumentNode.Descendants().Where(e => e.HasClass("movie-item")).ToList();
            var items = new List<Item>();
            foreach (var a in allTag_a)
            {
                var thumbImg = "";
                var thum = a.Descendants("div").FirstOrDefault(f => f.HasClass("public-film-item-thumb")); // orr contain style
                var style = thum.GetAttributeValue("style", "");
                if (style.HasValue())
                {
                    var styles = style.SplitSafe(";").FirstOrDefault(f => f.StartsWith("background-image"));//"background-image:url('data/MAD-031-2022-01-2.jpg')"
                    thumbImg = styles.Split("'")[1];
                }
                var item = new Item
                {
                    Title = a.GetAttributeValue("title", ""),
                    Href = buildLink(a.GetAttributeValue("href", "")),
                    Thumbnail = buildLink(thumbImg)
                };
                items.Add(item);


            }
            return items;
        }
        private string buildLink(string str)
        {
            return str.HasValue() ? $"{Uri.Scheme}://{Uri.Host}/{str}" : str;
        }

        public VideoItem GetVideo(Item itemList, out List<string> ctgs, out List<string> tags)
        {
            ctgs = new List<string>();
            tags = new List<string>();

            var doc = new HtmlWeb().Load(itemList.Href);
            var video_tag = doc.GetElementbyId("video");
            var videoId = video_tag.GetAttributeValue("data-id", "");
            if (videoId.HasValue())
            {
                var tag_ctgs = doc.DocumentNode.Descendants().Where(f => f.HasClass("category"));
                var tag_a_of_ctg = tag_ctgs.SelectMany(s => s.Descendants("a"));
                ctgs = tag_a_of_ctg.Select(s => s.InnerText.Trim()).Distinct().ToList();

                var self_tag = doc.DocumentNode.Descendants().Where(f => f.HasClass("tag-list"));
                tags = self_tag.SelectMany(s => s.Descendants("li").Select(ss => ss.InnerText.Trim())).Distinct().ToList();

                var kw = ctgs;
                kw.AddRange(tags);

                var content = doc.GetElementbyId("film-content-wrapper").InnerHtml;
                tags.AddRange(GetPublicCode(itemList.Title));
                tags.AddRange(GetPublicCode(content));

                var result = new VideoItem
                {
                    Url = $"//{Uri.Host}",
                    Data = videoId,
                    Title = itemList.Title,
                    MetaTitle = itemList.Title,
                    MetaKeywords = kw.Distinct().StrJoin(", "),
                    AllowComments = true,
                    Full = content,
                    MetaDescription = itemList.Title,
                };
                return result;
            }
            return null;
        }
        public IEnumerable<string> GetPublicCode(string str)
        {
            var rg = new Regex(@"\[.*\]");
            return rg.Matches(str).Select(s => s.Value.Trim());
        }


        public class Item
        {
            public string Title { get; set; }
            public string Href { get; set; }
            public string Thumbnail { get; set; }
        }
    }
}
