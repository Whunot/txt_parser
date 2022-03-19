using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SourceToMarkdown.Models;

namespace SourceToMarkdown
{
    class Program
    {
        #region CONSTANTS
        static (string, string) SECTION = ("РАЗДЕЛ ", "razdel");
        static (string, string) CHAPTER = ("Глава ", "glava");
        static (string, string) ITEM = ("Статья ", "statya");
        #endregion

        static void Main(string[] args)
        {

            //if (args.Length != 2)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.Error.WriteLine("Введите путь до .txt и путь развертки дерева каталогов.");
            //    Console.ForegroundColor = ConsoleColor.Gray;
            //    return;
            //}

            //var path = args[0];
            //var finalPath = args[1];

            var path = @"C:\Users\aestriplex\Desktop\documentomat-seo\tools\SourceToMarkdownTests\TestData\apk-rf-full.txt";
            var finalPath = @"C:\Users\aestriplex\Desktop\documentomat-seo\site\content\post";

            //  Построение дерева каталогов
            var sections = BuildCatalogTree(path);

            //  Непосредственная имплементация
            ImplementCatalogTree(finalPath, sections);

        }

        #region PRIVATE METHODS

        /// <summary>
        ///     Метод реализации соответствующего
        ///     сохраненному дереву каталогов
        /// </summary>
        private static void ImplementCatalogTree(string path, List<CatalogNode> sections)
        {
            foreach (var section in sections)
            {
                section.Implement(path);
            }
        }

        /// <summary>
        ///     Метод построения дерева каталогов
        /// </summary>
        private static List<CatalogNode> BuildCatalogTree(string path)
        {
            //  Дерево на выход
            var tree = new List<CatalogNode>();
            //  Корень
            var root = new CatalogComposite(tree);

            //  Считывание текст из файла .txt
            var stream = new StreamReader(path);
            var sections = stream.ReadToEnd().Split(SECTION.Item1).Skip(1);
            stream.Close();

            foreach (var section in sections)
            {
                //  Создание узла и его прикрепление к корневому узлу
                var sectionObj = new CatalogComposite(SECTION, section, root);
                tree.Add(sectionObj);

                //  Нарезка раздела на главы и проход по ним
                var chapters = section.Split(CHAPTER.Item1).Skip(1);

                if (chapters.Count() == 0)
                {
                    var items = section.Substring(section.IndexOf("\r\n")).Split(ITEM.Item1).Skip(1);

                    foreach (var item in items)
                    {
                        sectionObj.Add(new CatalogLeaf(ITEM, item, sectionObj));
                    }
                }

                foreach (var chapter in chapters)
                {
                    var chapterObj = new CatalogComposite(CHAPTER, chapter, sectionObj);
                    sectionObj.Add(chapterObj);

                    var items = chapter.Substring(chapter.IndexOf("\r\n")).Split(ITEM.Item1).Skip(1);

                    foreach (var item in items)
                    {
                        chapterObj.Add(new CatalogLeaf(ITEM, item, chapterObj));
                    }
                }
            }
            return tree;
        }

        #endregion
    }
}