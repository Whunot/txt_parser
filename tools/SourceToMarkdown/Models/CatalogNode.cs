using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace SourceToMarkdown.Models
{
    /// <summary>
    ///     Интерфейс, объявляющий общие свойства и методы
    ///     для объектов структуры
    /// </summary>
    public abstract class CatalogNode
    {

        #region protected props

        /// <summary>
        ///     Заголовок для данных
        /// </summary>
        protected virtual string? Title { get; set; }

        /// <summary>
        ///     Данные для создания файла .md
        /// </summary>
        protected virtual string? Data { get; set; }

        #endregion

        /// <summary>
        ///     Тип, название каталога или файла конечного
        ///     узла дерева
        /// </summary>
        public virtual string? Type { get; protected set; }

        /// <summary>
        ///     Порядковый номер узла
        /// </summary>
        public virtual int Number { get; protected set; }

        /// <summary>
        ///     Ссылка на родительский узел дерева
        /// </summary>
        public virtual CatalogComposite? FatherNode { get; protected set; }

        /// <summary>
        ///     Создание файла .md по заданному 
        ///     в аргументе пути (создает директории на пути,
        ///     если это не терминальный узел дерева)
        /// </summary>
        public abstract void Implement(string path);

        /// <summary>
        ///     Создает файл .md из строк Data
        /// </summary>
        public virtual void CreateMarkdownFile(string path)
        {
            using (var mdItemFile = File.Create(path))
            {
                var byteArray = new UTF8Encoding(true).GetBytes(Data);
                mdItemFile.Write(byteArray);
            }
        }

        /// <summary>
        ///     Метод, преобразующий строку в целочисленное значение
        ///     из римской или арабской записи
        /// </summary>
        protected virtual int ToInt(string str)
        {
            var romanDigits = new Dictionary<char, int> {{ 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 },
                                                         { 'C', 100 }, { 'D', 500 }, { 'M', 1000 }};

            if (!int.TryParse(str, out int result))
            {
                result = 0;

                for (var i = 0; i < str.Length - 1; i++)
                {
                    if (romanDigits[str[i]] < romanDigits[str[i + 1]])
                        result -= romanDigits[str[i]];
                    else
                        result += romanDigits[str[i]];
                }

                result += romanDigits[str[^1]];
            }

            return result;
        }

        /// <summary>
        ///     Возвращает ссылку на корень дерева
        /// </summary>
        protected virtual CatalogComposite GetRoot()
        {
            var root = FatherNode;

            while (root.FatherNode != null)
            {
                root = root.FatherNode;
            }

            return root;
        }

        /// <summary>
        ///     Поиск статьи по дереву
        /// </summary>
        protected virtual CatalogNode TreeTraversal(int num, string type)
        {
            //  Корень дерева, поиск ведется из корня
            var root = GetRoot();
            var sections = root.Childs;

            //  Если тип совпадает с разделом, достаточно выбрать необходимый элемент
            if (type == "razdel")
                return sections.Where(x => x.Number == num).First() as CatalogNode;

            //  Иначе проход по главам
            foreach (var section in sections)
            {
                var chapters = (section as CatalogComposite).Childs;

                var isSoughtChapterBetween = chapters.First().Number <= num && chapters.Last().Number >= num;

                //  Если искомый объект имеет тип "глава" и лежит в данной ветке,
                //  то метод возвращает его, иначе переход в следующую ветку
                if (type == "glava" && !isSoughtChapterBetween)
                    continue;
                else if (type == "glava" && isSoughtChapterBetween)
                    return chapters.Where(x => x.Number == num).First();

                //  Иначе проход по статьям
                foreach (var chapter in chapters)
                {
                    var items = (chapter is CatalogLeaf) ? chapters : (chapter as CatalogComposite).Childs;

                    var isSoightItemBetween = items.First().Number <= num && items.Last().Number >= num;

                    //  Если искомый объект "статья" и лежит в данной ветке,
                    //  метод возвращает его, иначе переход в следующую ветку
                    if (type == "statya" && isSoightItemBetween)
                        return items.Where(x => x.Number == num).First();
                }
            }

            return null;
        }
    }
}
