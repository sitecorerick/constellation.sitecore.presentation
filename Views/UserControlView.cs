// -----------------------------------------------------------------------------
// <copyright file="UserControlView.cs" company="genuine">
//      Copyright (c) @SitecoreDiamond. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------------
namespace Constellation.Sitecore.Views
{
    using Constellation.Sitecore.Items;
    using Constellation.Sitecore.Presenters;

    using global::Sitecore;
    using global::Sitecore.Data;
    using global::Sitecore.Data.Items;
    using global::Sitecore.Diagnostics;
    using global::Sitecore.Globalization;
    using global::Sitecore.Links;
    using global::Sitecore.Web.UI.WebControls;

    using Sitecore;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// A View based on Microsoft's UserControl, compatible with Sitecore's Sublayout objects. 
    /// T will be the Type used for the View's ViewModel, represented by the ViewModel property.
    /// </summary>
    /// <typeparam name="TModel">The Type to use for the View's ViewModel.</typeparam>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class UserControlView<TModel> : UserControl, IView<TModel>
        where TModel : class
    {
        /// <summary>
        /// The actual Sitecore WebControl that we wrap the IView around.
        /// Required because of the way Sitecore implements Sublayouts.
        /// </summary>
        private Sublayout sublayout;

        /// <summary>
        /// The model relevant to the view's context.
        /// </summary>
        private TModel model;

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the output can be cached.
        /// </summary>
        public bool Cacheable
        {
            get { return Sublayout.Cacheable; }
            set { Sublayout.Cacheable = value; }
        }

        /// <summary>
        /// Gets or sets a value to use as the "id" attribute on the outermost HTML container element.
        /// </summary>
        public string CssID { get; set; }

        /// <summary>
        /// Gets or sets a value to use as the "class" attribute on the outermost HTML container element.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets a value for the carousel color theme
        /// </summary>
        public string CarouselTheme { get; set; }

        /// <summary>
        /// Gets or sets the path or query to the Sitecore Item that the View is assigned
        /// </summary>
        public string DataSource
        {
            get { return Sublayout.DataSource; }
            set { Sublayout.DataSource = value; }
        }

        /// <summary>
        /// Gets the Database to be used to retrieve Items rendered by the View.
        /// </summary>
        public Database Database
        {
            get
            {
                return this.GetContextItem().Database;
            }
        }

        /// <summary>
        /// Gets the Language to be used for Items rendered by the View.
        /// </summary>
        public Language Language
        {
            get
            {
                return this.GetContextItem().Language;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to output any HTML if ViewModel is null.
        /// </summary>
        public bool RenderIfViewModelIsNull { get; set; }

        /// <summary>
        /// Gets an instance of T relevant to the view's context.
        /// </summary>
        /// <remarks>
        /// The name "ViewModel" was chosen to allow forward-compatibility with MVC, which 
        /// uses "Model". Also note that the MVC "Model" knows a bit more about the backend 
        /// than our "ViewModel", which is truly a dumb bucket.
        /// </remarks>
        public TModel ViewModel
        {
            get
            {
                return this.model ?? (this.model = this.Presenter.GetModel(this));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether output caching should be discrete for the Datasource.
        /// </summary>
        public bool VaryByData
        {
            get { return Sublayout.VaryByData; }
            set { Sublayout.VaryByData = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether output caching should be discrete for the Device.
        /// </summary>
        public bool VaryByDevice
        {
            get { return Sublayout.VaryByDevice; }
            set { Sublayout.VaryByDevice = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether output caching should be discrete for the authenticated user.
        /// </summary>
        public bool VaryByLogin
        {
            get { return Sublayout.VaryByLogin; }
            set { Sublayout.VaryByLogin = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether output caching should be discrete for runtime rendering parameters.
        /// </summary>
        public bool VaryByParm
        {
            get { return Sublayout.VaryByParm; }
            set { Sublayout.VaryByParm = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether output caching should be discrete for querystring parameters.
        /// </summary>
        public bool VaryByQueryString
        {
            get { return Sublayout.VaryByQueryString; }
            set { Sublayout.VaryByQueryString = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether output caching should be discrete for visitor sessions.
        /// </summary>
        public bool VaryByUser
        {
            get { return Sublayout.VaryByUser; }
            set { Sublayout.VaryByUser = value; }
        }

        /// <summary>
        /// Gets an instance of IPresenter capable of constructing an instance of TModel
        /// </summary>
        /// <remarks>
        /// This Property uses reflection internally to determine the correct presenter to
        /// load. If there is only one possible Presenter, override this property in your
        /// derived implementation and provide the correct presenter instance directly.
        /// </remarks>
        protected IPresenter<TModel> Presenter
        {
            get { return PresenterFactory.GetPresenter<TModel>(); }
        }

        /// <summary>
        /// Gets a Sitecore.Web.UI.WebControl we can use for this instance.
        /// When employed in a dynamically rendered user control, this is always the
        /// Parent control. If the user control is statically declared, we have to 
        /// create a control we can reference internally.
        /// </summary>
        protected Sublayout Sublayout
        {
            get
            {
                if (this.sublayout == null)
                {
                    if (this.Parent == null)
                    {
                        this.sublayout = new Sublayout();
                    }

                    this.sublayout = Parent as Sublayout;
                }

                return this.sublayout;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Either the resolved Datasource or the Context Item if the Datasource is empty.
        /// </summary>
        /// <returns>An Item.</returns>
        public Item GetItem()
        {
            if (DatasourceResolver.IsQuery(this.DataSource))
            {
                Item[] items = this.GetItems();
                if (items != null && items.Length > 0)
                {
                    return items[0];
                }
            }

            if (this.DataSource.Contains("."))
            {
                var context = this.GetContextItem();

                return DatasourceResolver.Resolve(this.DataSource.Replace(".", context.Paths.FullPath), context.Database);
            }

            if (this.DataSource.Contains("$site"))
            {
                return DatasourceResolver.Resolve(this.DataSource.Replace("$site", global::Sitecore.Context.Site.Name), global::Sitecore.Context.Database);
            }

            return this.GetSingleItemFromDataSource();
        }

        /// <summary>
        /// An array of either the resolved Datasource or the Context Item.
        /// </summary>
        /// <returns>An array of Items. The Array may be empty.</returns>
        public Item[] GetItems()
        {
            if (DatasourceResolver.IsQuery(this.DataSource))
            {
                var query = DatasourceResolver.EncodeQuery(this.DataSource);
                return this.GetContextItem().Database.SelectItems(query);
            }

            return new[] { this.GetItem() };
        }

        /// <summary>
        /// Returns the Context Item for the request.
        /// </summary>
        /// <returns>An Item.</returns>
        public Item GetContextItem()
        {
            return global::Sitecore.Context.Item;
        }

        /// <summary>
        /// Utilizes an appropriate LinkManager to resolve the item's browser URL.
        /// </summary>
        /// <param name="item">The item being linked.</param>
        /// <returns>A browser-ready URL.</returns>
        public string GetItemUrl(Item item)
        {
            return this.GetItemUrl(item.AsStronglyTyped());
        }

        /// <summary>
        /// Utilizes an appropriate LinkManager to resolve the item's browser URL.
        /// </summary>
        /// <param name="item">The item being linked.</param>
        /// <param name="options">The options for constructing the URL.</param>
        /// <returns>A browser-ready URL.</returns>
        public string GetItemUrl(Item item, UrlOptions options)
        {
            return this.GetItemUrl(item.AsStronglyTyped(), options);
        }

        /// <summary>
        /// Utilizes an appropriate LinkManager to resolve the item's browser URL.
        /// </summary>
        /// <param name="item">The item being linked.</param>
        /// <returns>A browser-ready URL.</returns>
        public string GetItemUrl(IStandardTemplate item)
        {
            var options = LinkManager.GetDefaultUrlOptions();
            options.Language = item.Language;
            return this.GetItemUrl(item, options);
        }

        /// <summary>
        /// Utilizes an appropriate LinkManager to resolve the item's browser URL.
        /// </summary>
        /// <param name="item">The item being linked.</param>
        /// <param name="options">The options for constructing the URL.</param>
        /// <returns>A browser-ready URL.</returns>
        public string GetItemUrl(IStandardTemplate item, UrlOptions options)
        {
            return this.GetLinkProvider().GetItemUrl(item.InnerItem, options);
        }

        /// <summary>
        /// Provides a quick way to access specific values in the querystring-style
        /// Parameters field on a Rendering definition.
        /// </summary>
        /// <returns>The value of the Parameters field in an indexable format.</returns>
        protected NameValueCollection GetParametersCollection()
        {
            if (string.IsNullOrEmpty(this.sublayout.Parameters))
            {
                return new NameValueCollection();
            }

            return HttpUtility.ParseQueryString(this.sublayout.Parameters);
        }

        /// <summary>
        /// Override to supply a LinkProvider other than the default LinkProvider.
        /// </summary>
        /// <returns>The link provider to use when resolving Item URLs.</returns>
        protected virtual LinkProvider GetLinkProvider()
        {
            return LinkManager.Provider;
        }

        /// <summary>
        /// Replicates the default functionality of WebControl.GetItem(), which is marked protected,
        /// so we can't access it from the Sublayout property.
        /// </summary>
        /// <returns>The Item to use as a rendering context.</returns>
        private Item GetSingleItemFromDataSource()
        {
            /*
             *  The following logic was pulled straight from Sitecore source code for WebControl.GetItem()
             *  IMHO the GetItem() method should be public rather than protected, but this is what
             *  we have to work with.
             */

            var contextItem = this.GetContextItem();
            var dataSource = this.DataSource;

            if (dataSource.Length > 0)
            {
                if (MainUtil.IsFullPath(dataSource))
                {
                    Database database = Sublayout.GetDatabase();
                    if (database != null)
                    {
                        contextItem = database.GetItem(dataSource);
                    }
                }
                else
                {
                    Assert.IsNotNull(contextItem, "Cannot resolve a relative data source path when the current item is null. Path is " + dataSource);
                    contextItem = contextItem.Axes.GetItem(dataSource);
                }
            }

            if (contextItem == null)
            {
                Tracer.Warning("Rendering \"" + this.GetType().FullName + "\" will not shown as the DataSource \"" + dataSource + "\" was not found.");
            }

            return contextItem;
        }
        #endregion
    }
}