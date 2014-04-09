// -----------------------------------------------------------------------------
// <copyright file="MetaLanguage.cs" company="genuine">
//      Copyright (c) @SitecoreDiamond. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------------
namespace Constellation.Sitecore.Views.HtmlHead
{
    using Constellation.Html;
    using Constellation.Sitecore.Items;
    using System.Web.UI;

    /// <summary>
    /// Renders a Meta element for the Language of the context Item.
    /// </summary>
    public class MetaLanguage : WebControlView<IStandardTemplate>
    {
        /// <summary>
        /// Renders the HTML output.
        /// </summary>
        /// <param name="output">The response writer.</param>
        protected override void RenderNormal(System.Web.UI.HtmlTextWriter output)
        {
            ////<META HTTP-EQUIV="Content-Language" CONTENT="en">

            output.RenderSelfClosingTag(
                HtmlTextWriterTag.Meta,
                new HtmlAttribute("http-equiv", "Content-Language"),
                new HtmlAttribute(HtmlTextWriterAttribute.Content, ViewModel.InnerItem.Language.Name));
        }
    }
}