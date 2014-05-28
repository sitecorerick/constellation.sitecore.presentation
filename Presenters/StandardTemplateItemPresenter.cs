namespace Constellation.Sitecore.Presenters
{
	using Constellation.Sitecore.Views;

	/// <summary>
	/// Converts the View's Datasource into the requested strongly-typed-Item.
	/// </summary>
	/// <typeparam name="TModel">The Type of Item to return.</typeparam>
	public class StandardTemplateItemPresenter<TModel> : IPresenter<TModel>
		where TModel : class
	{
		/// <summary>
		/// Coverts the results of View.GetItem() into the requested strongly-typed-item.
		/// </summary>
		/// <param name="view">The View requiring the Item.</param>
		/// <returns>An instance of TModel or null.</returns>
		public TModel GetModel(IView<TModel> view)
		{
			var item = view.GetItem();

			if (item == null)
			{
				return null;
			}

			var model = item.AsStronglyTyped(view.Language) as TModel;

			return model;
		}
	}
}
