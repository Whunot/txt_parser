using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SourceToMarkdown.Models
{
    /// <summary>
    ///     Класс составного узла дерева каталогов
    /// </summary>
    public class CatalogComposite : CatalogNode, IEnumerable
    {
        /// <summary>
        ///     Сыновья текущего составного узла
        /// </summary>
        public List<CatalogNode> Childs;

        /// <inheritdoc/>
        public override void Implement(string path)
        {
            var correctedPath = $"{path}/{Type}_{Number}";

            var dirInfo = new DirectoryInfo(correctedPath);
            if (!dirInfo.Exists)
                dirInfo.Create();

            CreateMarkdownFile(correctedPath + "/_index.md");

            foreach (var child in Childs)
            {
                child.Implement(correctedPath);
            }
        }

        /// <summary>
        ///     Конструктор составного узла
        ///     дерева каталогов
        /// </summary>
        public CatalogComposite((string, string) turple, string text, CatalogComposite fatherNode = null)
        {
            FatherNode = fatherNode;

            Number = ToInt(text.Substring(0, text.IndexOf(".")));

            Title = turple.Item1 + text.Substring(0, text.IndexOf("\r\n"));

            Data = $"---\n" +
                   $"title: \"{Title}\"\n" +
                   $"number: {Number}\n" +
                   $"date: \"{System.DateTime.Now}\"\n" +
                   $"draft: true\n" +
                   $"type: \"{turple.Item2}\"\n" +
                   $"---";

            Type = turple.Item2;

            Childs = new List<CatalogNode>();
        }

        /// <summary>
        ///     Конструктор корневого узла
        /// </summary>
        public CatalogComposite(List<CatalogNode> composites)
        {
            Number = 0;
            Childs = composites;
        }

        /// <summary>
        ///     Добавить сына в список сыновей составного узла
        /// </summary>
        public void Add(CatalogNode child)
        {
            Childs.Add(child);
        }

        /// <summary>
        ///     Итератор по сыновьям
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Childs).GetEnumerator();
        }
    }
}
