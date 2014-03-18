namespace Constellation.Sitecore.Presentation.Views
{
	using Constellation.Sitecore.Items;

	using global::Sitecore.Data;
	using global::Sitecore.Data.Items;
	using global::Sitecore.Globalization;
	using global::Sitecore.Links;


	/// <summary>
	/// The contract for Constellation.Sitecore.Presentation presentation components
	/// </summary>
	/// <typeparam name="TModel">The Model required by the View</typeparam>
	public interface IView<TModel>
	{
		/// <summary>
		/// Gets or sets a value indicating whether the output can be cached.
		/// </summary>
		bool Cacheable { get; set; }

		/// <summary>
		/// Gets or sets the path or query to the Sitecore Item that the View is assigned
		/// </summary>
		string DataSource { get; set; }

		/// <summary>
		/// Gets the Database to be used to retrieve Items rendered by the View.
		/// </summary>
		Database Database { get; }

		/// <summary>
		/// Gets the Language to be used for Items rendered by the View.
		/// </summary>
		Language Language { get; }

		/// <summary>
		/// Gets the TModel to render.
		/// </summary>
		TModel ViewModel { get; }

		/// <summary>
		/// Gets or sets a value indicating whether output caching should be discrete for the Datasource.
		/// </summary>
		bool VaryByData { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether output caching should be discrete for the Device.
		/// </summary>
		bool VaryByDevice { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether output caching should be discrete for the authenticated user.
		/// </summary>
		bool VaryByLogin { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether output caching should be discrete for runtime rendering parameters.
		/// </summary>
		bool VaryByParm { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether output caching should be discrete for querystring parameters.
		/// </summary>
		bool VaryByQueryString { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether output caching should be discrete for visitor sessions.
		/// </summary>
		bool VaryByUser { get; set; }

		/// <summary>
		/// Either the resolved Datasource or the Context Item if the Datasource is empty.
		/// </summary>
		/// <returns>An Item.</returns>
		Item GetItem();

		/// <summary>
		/// An array of either the resolved Datasource or the Context Item.
		/// </summary>
		/// <returns>An array of Items. The Array may be empty.</returns>
		Item[] GetItems();

		/// <summary>
		/// Returns the Context Item for the request.
		/// </summary>
		/// <returns>An Item.</returns>
		Item GetContextItem();

		/// <summary>
		/// Utilizes an appropriate LinkManager to resolve the item's browser URL.
		/// </summary>
		/// <param name="item">The item being linked.</param>
		/// <returns>A browser-ready URL.</returns>
		string GetItemUrl(Item item);

		/// <summary>
		/// Utilizes an appropriate LinkManager to resolve the item's browser URL.
		/// </summary>
		/// <param name="item">The item being linked.</param>
		/// <param name="options">The options for constructing the URL.</param>
		/// <returns>A browser-ready URL.</returns>
		string GetItemUrl(Item item, UrlOptions options);

		/// <summary>
		/// Utilizes an appropriate LinkManager to resolve the item's browser URL.
		/// </summary>
		/// <param name="item">The item being linked.</param>
		/// <returns>A browser-ready URL.</returns>
		string GetItemUrl(IStandardTemplate item);

		/// <summary>
		/// Utilizes an appropriate LinkManager to resolve the item's browser URL.
		/// </summary>
		/// <param name="item">The item being linked.</param>
		/// <param name="options">The options for constructing the URL.</param>
		/// <returns>A browser-ready URL.</returns>
		string GetItemUrl(IStandardTemplate item, UrlOptions options);
	}
}
