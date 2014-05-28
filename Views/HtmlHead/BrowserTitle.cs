namespace Constellation.Sitecore.Views.HtmlHead
{
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Changes the Title property of the .NET WebForm to match
	/// the appropriate value from the context item.
	/// </summary>
	/// <typeparam name="TModel">The model required by the view.</typeparam>
	public abstract class BrowserTitle<TModel> : WebControlView<TModel>
		where TModel : class
	{
		/// <summary>
		/// Called during the OnInit event.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
		protected new void OnInit(System.EventArgs e)
		{
			this.Cacheable = false;
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the output of the control.
		/// </summary>
		/// <param name="output">The response writer.</param>
		protected override void RenderNormal(System.Web.UI.HtmlTextWriter output)
		{
			this.Page.Title = this.GetBrowserTitle();
		}

		/// <summary>
		/// Gets the appropriate title value from the ViewModel.
		/// </summary>
		/// <returns>The browser title.</returns>
		protected abstract string GetBrowserTitle();
	}
}