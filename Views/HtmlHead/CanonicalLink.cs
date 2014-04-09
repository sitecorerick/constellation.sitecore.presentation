// -----------------------------------------------------------------------------
// <copyright file="CanonicalLink.cs" company="genuine">
//      Copyright (c) @SitecoreDiamond. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------------
namespace Constellation.Sitecore.Views.HtmlHead
{
    using Constellation.Html;
    using Constellation.Sitecore.Items;

    using global::Sitecore.Links;
    using global::Sitecore.Sites;

    /// <summary>
    /// Renders a link of type canonical to the official version of a given page.
    /// </summary>
    public abstract class CanonicalLink : WebControlView<IStandardTemplate>
    {
        /// <summary>
        /// Renders the HTML.
        /// </summary>
        /// <param name="output">The response writer.</param>
        protected override void RenderNormal(System.Web.UI.HtmlTextWriter output)
        {
            // <link rel="canonical" href="http://[hostname]" />
            output.RenderLink(this.GetLink(this.GetCanonicalSite(), this.GetLinkProvider()), "canonical");
        }

        /// <summary>
        /// Given the Model, return the correct site to use in link resolution.
        /// </summary>
        /// <returns>The correct Site to use.</returns>
        protected abstract SiteContext GetCanonicalSite();

        /// <summary>
        /// Given a site context and a link provider, create a URL representing the
        /// canonical reference to the given page.
        /// </summary>
        /// <param name="site">The site to use.</param>
        /// <param name="provider">The link provider to use.</param>
        /// <returns>The canonical URL.</returns>
        private string GetLink(SiteContext site, LinkProvider provider)
        {
            var options = provider.GetDefaultUrlOptions();
            options.Site = site;

            return provider.GetItemUrl(ViewModel.InnerItem, options);
        }
    }
}