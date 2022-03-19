using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SourceToMarkdown.Models
{
    /// <summary>
    ///     Класс конечного узла дерева каталогов
    /// </summary>
    public class CatalogLeaf : CatalogNode
    {
        /// <summary>
        ///     Непосредственно текст статьи
        /// </summary>
        protected string Text { get; set; }

        /// <inheritdoc/>
        public override void Implement(string path)
        {
            var correctedPath = $"{path}/{Type}_{Number}.md";

            Text = ReplaceToReferences(Text);

            Data += Text;

            CreateMarkdownFile(correctedPath);
        }

        /// <summary>
        ///     Конструктор конечного узла
        ///     дерева каталогов
        /// </summary>
        public CatalogLeaf((string, string) turple, string text, CatalogComposite fatherNode = null)
        {
            FatherNode = fatherNode;
            
            Number = ToInt(text.Substring(0, text.IndexOf(".")));

            Title = turple.Item1 + text.Substring(0, text.IndexOf("\r\n"));

            Text = text.Substring(text.IndexOf("\r\n"));

            Data = $"---\n" +
                   $"title: \"{Title}\"\n" +
                   $"title1: \"{Title.Split('.').First()}\"\n" +
                   $"number: {Number}\n" +
                   $"date: \"{DateTime.Now}\"\n" +
                   $"draft: true\n" +
                   $"type: \"{turple.Item2}\"\n" +
                   $"---\n";

            Type = turple.Item2;
        }

        /// <summary>
        ///     Заменяет упоминания статей на ссылки
        ///     на соответствующие статьи
        /// </summary>
        private string ReplaceToReferences(string text)
        {
            var pattern = @"(стать[а-я]+) (\d+)|(глав[а-я]+) (\d+)|(раздел[а-я]+) ([IVXLCDM]+)"; //(стать[а-я]+) (\d+).{1,5}[^\d](\d+)| (\d+)|(глав[а-я]+) (\d+)|(раздел[а-я]+) (I+)

            var input = text.Substring(text.IndexOf("\r\n"));

            var output = Regex.Replace(input, pattern,
                delegate (Match match)
                {
                    if (match.Groups[2].Length != 0)
                    {
                        var item = TreeTraversal(int.Parse(match.Groups[2].Value), "statya");
                        var chapter = item.FatherNode;
                        var section = chapter.FatherNode;

                        var itemNodeName = $"{item.Type}_{ item.Number}";
                        var chapterNodeName = $"{chapter.Type}_{chapter.Number}";
                        var sectionNodeName = $"{section.Type}_{section.Number}";

                        var originalLine = match.Groups[1].ToString();
                        var originalReferenceLine = match.Groups[2].ToString();

                        return $"{originalLine} [{originalReferenceLine}](/post/{sectionNodeName}/{chapterNodeName}/{itemNodeName})";
                    }
                    else if (match.Groups[4].Length != 0)
                    {
                        var chapter = TreeTraversal(int.Parse(match.Groups[4].Value), "glava");
                        var section = chapter.FatherNode;

                        var chapterNodeName = $"{chapter.Type}_{chapter.Number}";
                        var sectionNodeName = $"{section.Type}_{section.Number}";

                        var originalLine = match.Groups[3].ToString();
                        var originalReferenceLine = match.Groups[4].ToString();

                        return $"{originalLine} [{originalReferenceLine}](/post/{sectionNodeName}/{chapterNodeName})";
                    }
                    else
                    {
                        var section = TreeTraversal(ToInt(match.Groups[6].Value), "razdel");

                        var sectionNodeName = $"{section.Type}_{section.Number}";

                        var originalLine = match.Groups[5].ToString();
                        var originalReferenceLine = match.Groups[6].ToString();

                        return $"{originalLine} [{originalReferenceLine}](/post/{sectionNodeName})";
                    }
                });

            return output;
        }
    }
}
