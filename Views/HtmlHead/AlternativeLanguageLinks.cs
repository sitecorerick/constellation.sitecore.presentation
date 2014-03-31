// -----------------------------------------------------------------------------
// <copyright file="AlternativeLanguageLinks.cs" company="genuine">
//      Copyright (c) @SitecoreDiamond. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------------
namespace Constellation.Sitecore.Presentation.Views.HtmlHead
{
    using Constellation.Html;
    using Constellation.Sitecore.Items;
    using global::Sitecore.Data.Items;
    using global::Sitecore.Globalization;
    using global::Sitecore.Sites;
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    /// <summary>
    /// Renders an alternate link for each language version of the context item.
    /// </summary>
    public abstract class AlternativeLanguageLinks : WebControlView<IStandardTemplate>
    {
        /// <summary>
        /// Writes the HTML.
        /// </summary>
        /// <param name="output">The response writer.</param>
        protected override void RenderNormal(System.Web.UI.HtmlTextWriter output)
        {
            var languages = this.GetSiteLanguages();

            foreach (var language in languages)
            {
                if (language.Name.Equals(global::Sitecore.Context.Language.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var item = ViewModel.InnerItem.GetBestFitLanguageVersion(language);
                if (!item.LanguageVersionIsEmpty())
                {
                    this.RenderLanguageLink(output, this.GetSiteContext(), item);
                }
            }
        }

        /// <summary>
        /// Return the list of languages supported by the context site. These will be
        /// used to render links for the context Item in those languages.
        /// </summary>
        /// <returns>A list of languages to use.</returns>
        protected abstract ICollection<Language> GetSiteLanguages();

        /// <summary>
        /// Returns a <see cref="SiteContext"/> to use for link resolution.
        /// </summary>
        /// <returns>
        /// The <see cref="SiteContext"/> to use for link resolution.
        /// </returns>
        protected abstract SiteContext GetSiteContext();

        /// <summary>
        /// Writes the link tag for the given Item.
        /// </summary>
        /// <param name="output">The response writer.</param>
        /// <param name="site">The site context.</param>
        /// <param name="item">The Item to parse.</param>
        private void RenderLanguageLink(HtmlTextWriter output, SiteContext site, Item item)
        {
            var provider = this.GetLinkProvider();
            var options = provider.GetDefaultUrlOptions();
            options.Site = site;
            options.Language = item.Language;

            var url = provider.GetItemUrl(item, options);

            // <link rel="alternate" hreflang="[Language ISO Code]" href="http://[hostname ISO Code]/[Page Path]" />
            output.RenderLink(
                new HtmlAttribute(HtmlTextWriterAttribute.Rel, "alternate"),
                new HtmlAttribute("hreflang", item.Language.Name),
                new HtmlAttribute(HtmlTextWriterAttribute.Href, url));
            output.WriteLine();
        }
    }
}