using System.Web;

namespace Constellation.Sitecore.Presentation.Presenters
{
	using Constellation.Sitecore.Presentation.Views;

	/// <summary>
	/// A Presenter that caches its model in the Request.Items collection
	/// to make it available for subsequent calls during a particular
	/// request.
	/// </summary>
	/// <typeparam name="TModel">The Model required by the view.</typeparam>
	public abstract class PresenterWithRequestCachedModel<TModel> : IPresenter<TModel>
		where TModel : class
	{
		/// <summary>
		/// Creates a TModel based upon the Datasource information supplied by the View.
		/// </summary>
		/// <param name="view">The view instance requireing a model.</param>
		/// <returns>An instance of TModel or null.</returns>
		public TModel GetModel(IView<TModel> view)
		{
			if (!this.ShouldCreateModel(view))
			{
				return null;
			}

			TModel model;
			var key = this.GetCacheKey(view);
			if (!string.IsNullOrEmpty(key))
			{
				model = this.GetModelUsingCache(key, view);
			}
			else
			{
				model = this.CreateModel(view);
			}

			return model;
		}

		/// <summary>
		/// Returns a key to use for retrieving or storing the model in
		/// a cache.
		/// </summary>
		/// <param name="view">The view to parse.</param>
		/// <returns>A cache key.</returns>
		protected virtual string GetCacheKey(IView<TModel> view)
		{
			var item = view.GetItem();

			return this.GetType() + item.ID.ToString();
		}

		/// <summary>
		/// Determine if a model should be created for the view based upon
		/// the view's properties.
		/// </summary>
		/// <param name="view">The view to parse.</param>
		/// <returns>True if a model should be created.</returns>
		protected abstract bool ShouldCreateModel(IView<TModel> view);

		/// <summary>
		/// Create a model using the view.
		/// </summary>
		/// <param name="view">The view to parse.</param>
		/// <returns>A populated instance of TModel.</returns>
		protected abstract TModel CreateModel(IView<TModel> view);

		/// <summary>
		/// Attempts to get the model from the cache. If it can't,
		/// it createa a new model and caches it.
		/// </summary>
		/// <param name="key">The cache key.</param>
		/// <param name="view">The view needed to create a new model.</param>
		/// <returns>An instance of TModel.</returns>
		private TModel GetModelUsingCache(string key, IView<TModel> view)
		{
			var model = HttpContext.Current.Items[key] as TModel;

			if (model == null)
			{
				model = CreateModel(view);

				if (model != null)
				{
					HttpContext.Current.Items.Add(key, model);
				}
			}

			return model;
		}
	}
}