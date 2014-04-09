// -----------------------------------------------------------------------------
// <copyright file="WebControlView.cs" company="genuine">
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
    using global::Sitecore.Globalization;
    using global::Sitecore.Links;
    using global::Sitecore.Sites;

    using Sitecore;
    using System.Collections.Specialized;
    using System.Web;

    /// <summary>
    /// A View based on Sitecore's WebControl. T will be the Type used for the View's
    /// ViewModel, represented by the ViewModel property.
    /// </summary>
    /// <typeparam name="TModel">The Type to use for the View's ViewModel.</typeparam>
    public abstract class WebControlView<TModel> : global::Sitecore.Web.UI.WebControl, IView<TModel>
        where TModel : class
    {
        #region Fields
        /// <summary>
        /// The model relevant to the view's context.
        /// </summary>
        private TModel model;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value to use as the "id" attribute on the outermost HTML container element.
        /// </summary>
        public string CssID { get; set; }

        /// <summary>
        /// Gets the caching id.
        /// </summary>
        public override string CachingID
        {
            get
            {
                if (string.IsNullOrEmpty(this.UniqueID))
                {
                    return this.GetType().FullName;
                }

                return this.UniqueID;
            }
        }

        /// <summary>
        /// Gets the Database to be used to retrieve Items rendered by the View.
        /// </summary>
        public new Database Database
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
        /// Gets or sets a value indicating whether to output any HTML if ViewModel is null.
        /// </summary>
        public bool RenderIfViewModelIsNull { get; set; }

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
        #endregion

        #region Methods
        /// <summary>
        /// The Sitecore Item referenced by the DataSource property of the control.
        /// If the DataSource is a query, the first Item in the result set will be returned.
        /// </summary>
        /// <returns>A Sitecore Item instance.</returns>
        public new Item GetItem()
        {
            if (this.DataSource.Contains("$site"))
            {
                this.DataSource = this.DataSource.Replace("$site", global::Sitecore.Context.Site.Name);
            }

            if (DatasourceResolver.IsQuery(this.DataSource))
            {
                Item[] items = this.GetItems();
                if (items != null && items.Length > 0)
                {
                    return items[0];
                }
            }

            if (DataSource.Contains("."))
            {
                var context = this.GetContextItem();

                return DatasourceResolver.Resolve(DataSource.Replace(".", context.Paths.FullPath), context.Database);
            }

            return base.GetItem();
        }

        /// <summary>
        /// The Sitecore Items referenced by the DataSource property of the control.
        /// If the DataSource references a single item, the resulting array will contain that
        /// item.
        /// </summary>
        /// <returns>An array of Sitecore Item instances.</returns>
        public Item[] GetItems()
        {
            if (DatasourceResolver.IsQuery(this.DataSource))
            {
                string query = DatasourceResolver.EncodeQuery(this.DataSource);
                return GetContextItem().Database.SelectItems(query);
            }

            return new[] { this.GetItem() };
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
            if (item == null)
            {
                return null;
            }

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
        /// The get cache key.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string GetCacheKey()
        {
            SiteContext site = global::Sitecore.Context.Site;
            if (site == null)
            {
                return string.Empty;
            }

            if (!this.Cacheable)
            {
                return string.Empty;
            }

            if (this.SkipCaching())
            {
                return string.Empty;
            }

            if (!site.CacheHtml)
            {
                return string.Empty;
            }

            string str1 = this.CachingID;
            if (str1.Length == 0)
            {
                str1 = this.CacheKey;
            }

            if (str1.Length > 0)
            {
                string str2 = str1 + "_#lang:" + Language.Current.Name.ToUpperInvariant();
                if (this.VaryByData)
                {
                    string str3 = this.DataSource;
                    if (str3.Length == 0)
                    {
                        str3 = global::Sitecore.Context.Item.Paths.Path;
                    }

                    str2 = str2 + "_#data:" + str3;
                }

                if (this.VaryByDevice)
                {
                    str2 = str2 + "_#dev:" + global::Sitecore.Context.GetDeviceName();
                }

                if (this.VaryByLogin)
                {
                    str2 = str2 + "_#login:" + (global::Sitecore.Context.IsLoggedIn ? 1 : 0);
                }

                if (this.VaryByUser)
                {
                    str2 = str2 + "_#user:" + global::Sitecore.Context.GetUserName();
                }

                if (this.VaryByParm)
                {
                    str2 = str2 + "_#parm:" + this.Parameters;
                }

                if (this.VaryByQueryString)
                {
                    SiteRequest request = site.Request;
                    if (request != null)
                    {
                        str2 = str2 + "_#qs:" + MainUtil.ConvertToString(request.QueryString, "=", "&");
                    }
                }

                return str2;
            }

            return string.Empty;
        }

        /// <summary>
        /// Implementation of Sitecore's contract from WebControl. This is decorated with logic
        /// that allows the control to gracefully hide if no data is available for rendering.
        /// </summary>
        /// <param name="output">The HtmlTextWriter for the response.</param>
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            if (global::Sitecore.Context.Site != null && global::Sitecore.Context.PageMode.IsPageEditorEditing)
            {
                if (this.ViewModel == null)
                {
                    var sourceString = "context item";

                    if (!string.IsNullOrEmpty(this.DataSource))
                    {
                        sourceString = this.DataSource;
                    }

                    output.Write("[Rendering {0} is not compatible with datasource: {1}.]", this.GetType().Name, sourceString);
                }

                this.RenderPageEditorEditing(output);
            }
            else
            {
                if (this.RenderIfViewModelIsNull || this.ViewModel != null)
                {
                    this.RenderNormal(output);
                }
                else
                {
                    this.Visible = false;
                }
            }
        }

        /// <summary>
        /// Provides a quick way to access specific values in the querystring-style
        /// Parameters field on a Rendering definition.
        /// </summary>
        /// <returns>The value of the Parameters field in an indexable format.</returns>
        protected NameValueCollection GetParametersCollection()
        {
            if (string.IsNullOrEmpty(this.Parameters))
            {
                return new NameValueCollection();
            }

            return HttpUtility.ParseQueryString(this.Parameters);
        }

        /// <summary>
        /// Called from Sitecore's DoRender() contract, this method will be executed if the control
        /// is not cached and the control has data to render, unless in Page Editor, in which case 
        /// the logic will execute regardless.
        /// </summary>
        /// <param name="output">The HtmlTextWriter for the response.</param>
        protected abstract void RenderNormal(System.Web.UI.HtmlTextWriter output);

        /// <summary>
        /// Called from Sitecore's DoRender() contract if the site is in PageEditorEditing mode.
        /// By default, calls RenderModel without the null check. Override locally with appropriate
        /// functionality.
        /// </summary>
        /// <param name="output">The HtmlTextWriter for the response.</param>
        protected virtual void RenderPageEditorEditing(System.Web.UI.HtmlTextWriter output)
        {
            this.RenderNormal(output);
        }

        /// <summary>
        /// Override to supply a LinkProvider other than the default LinkProvider.
        /// </summary>
        /// <returns>The link provider to use when resolving Item URLs.</returns>
        protected virtual LinkProvider GetLinkProvider()
        {
            return LinkManager.Provider;
        }
        #endregion
    }
}
