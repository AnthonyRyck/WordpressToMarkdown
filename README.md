# WordpressToMarkdown

[Français](https://github.com/AnthonyRyck/WordpressToMarkdown/blob/main/README.md#Utilisations)

-----

C'est une librairie qui permet de convertir un flux JSON d'un site wordpress en MarkDown.  


## Utilisations

Soit il faut donner une URL d'un site (*[voir ici pour les infos]()*) à la `class WordpressCollector` qui va récupèrer le flux JSON et le convertir en markdown, comme par exemple :

```csharp
// Récupère les 5 derniers articles
string urlWordpress = "https://www.ctrl-alt-suppr.dev/wp-json/wp/v2/posts?per_page=5";

WordpressCollector converterWordpress = new WordpressCollector();
List<MarkdownResult> resultConvertMd = await converter.GetPosts(urlWordpress);
```
ou alors passer directement le contenu à la `class ConverterWordpress` :
```csharp
ConverterWordpress convert = new ConverterWordpress();
string markdown = await convert.ConvertToMarkdownAsync("Titre de l'article", contenuBrut);
```

Il est possible de fournir ces propres transformations. Il faut implémenter l'interface `ITransformWordpress` :

```csharp
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
```
Ensuite, reste plus qu'à fournir vos transformations à la `class ConverterWordpress` sur la méthode :
```csharp
public Task<string> ConvertToMarkdownAsync(string title, string contentPost, params ITransformWordpress[] transformations);
```

## Problème connu
Pour la conversion, j'utilise la librairie [HtmlAgilityPack](https://html-agility-pack.net/), et il arrive qu'elle ajoute des "balises" fermantes sur des portions de code.  
Comme par exemple :  
```csharp
namespace WebApiGraphQl.Data
{
    public interface IDataAccess
    {
	/// <summary>
	/// Retourne toutes personnes
	/// </summary>
	/// <returns></returns>
	IEnumerable<Personne> GetAll();

	/// <summary>
	/// Retourne la personne par rapport à son ID
	/// </summary>
	/// <param name="id" />
	/// <returns></returns>
	Personne GetPersonne(Guid id);
    }
}</Personne>
```
Elle a ajouté `</Personne>` car elle considère `IEnumerable<Personne>` comme une balise ouvrante et elle ferme automatiquement, c'est pour cela que ce n'est pas parfait, il peut y avoir quelques retouches.

## URL Wordpress JSON  

Pour connaitre l'URL d'un site wordpress, il faut prendre cette base :  
`https://url-du-wordpress/wp-json/wp/v2/`  
Ensuite, pour récupérer plusieurs posts d'un coup :  
`https://url-du-wordpress/wp-json/wp/v2/posts?per_page=5`  
Et pour avoir un post en particulier, il faut :  
`http://url-du-wordpress/wp-json/wp/v2/posts?slug=TITRE-DU-POST`  


-----
