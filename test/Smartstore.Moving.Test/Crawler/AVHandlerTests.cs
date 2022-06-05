using NUnit.Framework;
using Smartstore.Core.Identity;
using Smartstore.Moving.Tasks;
using Smartstore.Test.Common;

namespace Smartstore.Moving.Test.Crawler
{
    [TestFixture]
    public class AVHandlerTests
    {
        private Customer _customer; 

        [SetUp]
        public virtual void Setup()
        {
            _customer = new Customer();
        }
        [Test]
        public void Crawl_List()
        {
            AVHandler avHandler = new AVHandler("https://javhd.onl/search/china/page/1");

            var items = avHandler.CrawlPageList();
            Assert.IsTrue(items.Count > 0);
        }
    }
}