using Smartstore.Collections;
using Smartstore.Core.Content.Menus;
using Smartstore.Web.Rendering.Builders;

namespace Smartstore.Moving
{
    public class AdminMenu : AdminMenuProvider
    {
        protected override void BuildMenuCore(TreeNode<MenuItem> modulesNode)
        {
            // Insert menu items for list views.
            var blogMenuItem = new MenuItem().ToBuilder()
                .ResKey("Admin.Moving.Video")
                .Icon("rss", "bi")
                .PermissionNames(MovingPermissions.Self)
                .AsItem();

            var blogPostsMenuItem = new MenuItem().ToBuilder()
                .ResKey("Admin.Moving.Video.Video")
                .Action("List", "Video", new { area = "Admin" })
                .AsItem();

            var videoMenuItem = new MenuItem().ToBuilder()
                .ResKey("Admin.Moving.Video.VideoTags")
                .Action("Comments", "Video", new { area = "Admin" })
                .AsItem();

            var blogNode = new TreeNode<MenuItem>(blogMenuItem);
            var parent = modulesNode.Root.SelectNodeById("cms");
            var refNode = parent.SelectNodeById("topics") ?? parent.SelectNodeById("menus");

            blogNode.InsertAfter(refNode);

            var blogPostsNode = new TreeNode<MenuItem>(blogPostsMenuItem);
            var blogCommentsNode = new TreeNode<MenuItem>(videoMenuItem);

            blogNode.Append(blogPostsNode);
            blogNode.Append(blogCommentsNode);
        }
    }
}
