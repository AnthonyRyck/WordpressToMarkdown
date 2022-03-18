using MarkdownWordpress.ModelsValidation;
using Microsoft.AspNetCore.Components;
using WordpressToMarkdown.Models;

namespace MarkdownWordpress.ViewModels
{
	public interface IMainViewModel
	{
		/// <summary>
		/// URL d'un site Wordpress
		/// </summary>
		WordpressSiteValidation Url { get; set; }

		/// <summary>
		/// Liste des résulats de conversion
		/// </summary>
		List<MarkdownResult> ResultConvertMd { get; set; }

		/// <summary>
		/// Pour l'affichage du résultat.
		/// </summary>
		MarkdownResult ResultMarkdownSelected { get; set; }

		/// <summary>
		/// Valide la saisie
		/// </summary>
		void OnValidSubmitUrl();

		/// <summary>
		/// Permet de télécharger les posts en MD.
		/// </summary>
		/// <returns></returns>
		Task Download();

		/// <summary>
		/// Lors de la sélection d'un résultat.
		/// </summary>
		void OnSelectResultMd(ChangeEventArgs e);

		/// <summary>
		/// Pour avoir accès au state dans la viewmodel
		/// </summary>
		/// <param name="state"></param>
		void SetStateHasChanged(Action state);
	}
}
