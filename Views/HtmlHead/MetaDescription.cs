// -----------------------------------------------------------------------------
// <copyright file="MetaDescription.cs" company="genuine">
//      Copyright (c) @SitecoreDiamond. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------------
namespace Constellation.Sitecore.Views.HtmlHead
{
    /// <summary>
    /// Renders a meta tag for description.
    /// </summary>
    /// <typeparam name="TModel">The model to use.</typeparam>
    public abstract class MetaDescription<TModel> : MetaTag<TModel>
        where TModel : class
    {
        /// <summary>
        /// Return the value for the name attribute.
        /// </summary>
        /// <returns>The name.</returns>
        protected override string GetName()
        {
            return "description";
        }

        /// <summary>
        /// Return the value for the content attribute.
        /// </summary>
        /// <returns>The content value.</returns>
        protected override string GetContent()
        {
            return this.GetDescription();
        }

        /// <summary>
        /// Return the appropriate Description string from the ViewModel.
        /// </summary>
        /// <returns>The string to use for the description.</returns>
        protected abstract string GetDescription();
    }
}