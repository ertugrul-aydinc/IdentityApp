using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCoreIdentity.App.Web.TagHelpers
{
    public class UserPictureThumbnailTagHelper : TagHelper
    {
        public string? PictureUrl { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";

            if (string.IsNullOrWhiteSpace(PictureUrl))
            {
                output.Attributes.SetAttribute("src", "userpictures/default_user_image.png");
            }
            else
            {
                output.Attributes.SetAttribute("src", $"userpictures/{PictureUrl}");
            }
        }
    }
}
