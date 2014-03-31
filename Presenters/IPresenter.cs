// -----------------------------------------------------------------------------
// <copyright file="IPresenter.cs" company="genuine">
//      Copyright (c) @SitecoreDiamond. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------------
namespace Constellation.Sitecore.Presentation.Presenters
{
    using Constellation.Sitecore.Presentation.Views;

    /// <summary>
    /// The contract for Presenters.
    /// </summary>
    /// <typeparam name="TModel">The Model required by the View.</typeparam>
    public interface IPresenter<TModel> where TModel : class
    {
        /// <summary>
        /// Creates a TModel based upon the Datasource information supplied by the View.
        /// </summary>
        /// <param name="view">The view instance requiring a model.</param>
        /// <returns>An instance of TModel or null.</returns>
        TModel GetModel(IView<TModel> view);
    }
}
