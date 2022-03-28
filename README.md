# WordpressToMarkdown

[![NuGet](https://img.shields.io/nuget/vpre/WordpressToMarkdown.svg)](https://www.nuget.org/packages/WordpressToMarkdown/)
[![Downloads](https://img.shields.io/nuget/dt/WordpressToMarkdown.svg)](https://www.nuget.org/packages/WordpressToMarkdown)

[Français](https://github.com/AnthonyRyck/WordpressToMarkdown/blob/main/README.md#Utilisations)  
[English](https://github.com/AnthonyRyck/WordpressToMarkdown/blob/main/README.md#Use)

-----

C'est une librairie qui permet de convertir un flux JSON d'un site wordpress en MarkDown.  
Un live sur [wp-to-markdown.ctrl-alt-suppr.dev](https://wp-to-markdown.ctrl-alt-suppr.dev/)


## Utilisations

Soit il faut donner une URL d'un site (*[voir ici pour les infos](https://github.com/AnthonyRyck/WordpressToMarkdown/blob/main/README.md#URL)*) à la `class WordpressCollector` qui va récupèrer le flux JSON et le convertir en markdown, comme par exemple :

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

## Conversion
Pour les blocks de code, j'utilise sur Wordpress l'extension [Enlighter](https://wordpress.org/plugins/enlighter/), du coup mes transformations sont basées sur certains "mots clés" de cette extension.  
Par exemple, pour avoir le langage, je prend sur l'attribut : `data-enlighter-language`.  

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

## URL
Wordpress JSON  

Pour connaitre l'URL d'un site wordpress, il faut prendre cette base :  
`https://url-du-wordpress/wp-json/wp/v2/`  
Ensuite, pour récupérer plusieurs posts d'un coup :  
`https://url-du-wordpress/wp-json/wp/v2/posts?per_page=5`  
Et pour avoir un post en particulier, il faut :  
`http://url-du-wordpress/wp-json/wp/v2/posts?slug=TITRE-DU-POST`  


-----

## Use

Either you have to give a URL of a site (*[see here for info](https://github.com/AnthonyRyck/WordpressToMarkdown/blob/main/README.md#URL-JSON)*) à la `class WordpressCollector` to the `WordpressCollector class` which will retrieve the JSON stream and convert it into a markdown, such as :  

```csharp
// Get the 5 last posts
string urlWordpress = "https://www.ctrl-alt-suppr.dev/wp-json/wp/v2/posts?per_page=5";

WordpressCollector converterWordpress = new WordpressCollector();
List<MarkdownResult> resultConvertMd = await converter.GetPosts(urlWordpress);
```
or go directly to the `ConverterWordpress class` :  
```csharp
ConverterWordpress convert = new ConverterWordpress();
string markdown = await convert.ConvertToMarkdownAsync("Titre de l'article", contenuBrut);
```

It is possible to provide these own transformations. It is necessary to implement the interface `ITransformWordpress`:  

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
Then, it remains more than to provide your transformations to the `ConverterWordpress class` on the method :  
```csharp
public Task<string> ConvertToMarkdownAsync(string title, string contentPost, params ITransformWordpress[] transformations);
```

## URL-JSON  

To know the URL of a wordpress site, you have to take this basis :  
`https://url-du-wordpress/wp-json/wp/v2/`  
Then, to retrieve several posts at once :  
`https://url-du-wordpress/wp-json/wp/v2/posts?per_page=5`  
And to have a particular post, you have to :  
`http://url-du-wordpress/wp-json/wp/v2/posts?slug=TITRE-DU-POST`  
