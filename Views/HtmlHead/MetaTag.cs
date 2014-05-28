namespace Constellation.Sitecore.Views.HtmlHead
{
	using Constellation.Html;

	/// <summary>
	/// Renders a meta tag.
	/// </summary>
	/// <typeparam name="TModel">The model to use.</typeparam>
	public abstract class MetaTag<TModel> : WebControlView<TModel>
		where TModel : class
	{
		/// <summary>
		/// Renders the HTML
		/// </summary>
		/// <param name="output">The response writer.</param>
		protected override void RenderNormal(System.Web.UI.HtmlTextWriter output)
		{
			output.RenderMeta(this.GetName(), this.GetContent());
		}

		/// <summary>
		/// Return the value for the name attribute.
		/// </summary>
		/// <returns>The name.</returns>
		protected abstract string GetName();

		/// <summary>
		/// Return the value for the content attribute.
		/// </summary>
		/// <returns>The content value.</returns>
		protected abstract string GetContent();
	}
}