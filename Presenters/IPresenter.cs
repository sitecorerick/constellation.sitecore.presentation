using Constellation.Sitecore.Presentation.Views;

namespace Constellation.Sitecore.Presentation.Presenters
{
	/// <summary>
	/// The contract for Presenters.
	/// </summary>
	/// <typeparam name="TModel">The Model required by the View.</typeparam>
	public interface IPresenter<TModel>
	{
		/// <summary>
		/// Creates a TModel based upon the Datasource information supplied by the View.
		/// </summary>
		/// <param name="view">The view instance requireing a model.</param>
		/// <returns>An instance of TModel or null.</returns>
		TModel GetModel(IView<TModel> view);
	}
}
