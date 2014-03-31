// -----------------------------------------------------------------------------
// <copyright file="PresenterFactory.cs" company="genuine">
//      Copyright (c) @SitecoreRick. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------------
namespace Constellation.Sitecore.Presentation
{
    using Constellation.Sitecore.Items;
    using Constellation.Sitecore.Presentation.Presenters;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Inspects the currently running application for implementations of IPresenter.
    /// </summary>
    public static class PresenterFactory
    {
        #region Fields
        /// <summary>
        /// The list of candidate IPresenter implementations.
        /// </summary>
        private static CandidateTypeList presenterTypes;

        /// <summary>
        /// Gets the list of candidate IPresenter implementations.
        /// </summary>
        public static IEnumerable<Type> PresenterTypes
        {
            get
            {
                if (presenterTypes == null)
                {
                    presenterTypes = new CandidateTypeList(typeof(IPresenter<>));
                }

                return presenterTypes.CandidateTypes;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Inspects the currently running application for implementations of IPresenter and
        /// returns an instance of IPresenter where the generic property matches the requirements
        /// of the view.
        /// </summary>
        /// <typeparam name="TModel">The type of the Model required by the View.</typeparam>
        /// <returns>An instance of IPresenter&lt;T&gt;.</returns>
        public static IPresenter<TModel> GetPresenter<TModel>()
            where TModel : class
        {
            /*
             * First figure out if it's an IStandardItem model, in which case, we can
             * just return the default presenter
             */
            if (typeof(IStandardTemplate).IsAssignableFrom(typeof(TModel)))
            {
                return GetDefaultPresenter<TModel>();
            }

            // We go wading through the IPresenter candidates looking for a match.
            var desiredType = typeof(IPresenter<TModel>);
            foreach (var availableType in PresenterTypes)
            {
                if (desiredType.IsAssignableFrom(availableType))
                {
                    return Activator.CreateInstance(availableType) as IPresenter<TModel>;
                }
            }

            return GetDefaultPresenter<TModel>();
        }

        /// <summary>
        /// Provides a basic Presenter.
        /// </summary>
        /// <typeparam name="TModel">The model Type the presenter must support.</typeparam>
        /// <returns>An instance of IPresenter&gt;T&lt;.</returns>
        public static IPresenter<TModel> GetDefaultPresenter<TModel>()
            where TModel : class
        {
            return new StandardTemplateItemPresenter<TModel>();
        }
        #endregion
    }
}
