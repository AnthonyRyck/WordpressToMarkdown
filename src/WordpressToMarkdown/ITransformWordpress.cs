namespace WordpressToMarkdown
{
	public interface ITransformWordpress
	{
		/// <summary>
		/// Récupère le contenu en string, effectu les transformations
		/// et retourne le contenu.
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		string Transform(string content);
	}
}
