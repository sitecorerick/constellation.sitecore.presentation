namespace Constellation.Sitecore.Views.HtmlHead
{
	/// <summary>
	/// Renders a meta keywords element.
	/// </summary>
	/// <typeparam name="TModel">The model to use.</typeparam>
	public abstract class MetaKeywords<TModel> : MetaTag<TModel>
		where TModel : class
	{
		/// <summary>
		/// Return the value for the name attribute.
		/// </summary>
		/// <returns>The name.</returns>
		protected override string GetName()
		{
			return "keywords";
		}

		/// <summary>
		/// Return the value for the content attribute.
		/// </summary>
		/// <returns>The content value.</returns>
		protected override string GetContent()
		{
			return this.GetKeywords();
		}

		/// <summary>
		/// Return the keywords to use.
		/// </summary>
		/// <returns>The keywords.</returns>
		protected abstract string GetKeywords();
	}
}