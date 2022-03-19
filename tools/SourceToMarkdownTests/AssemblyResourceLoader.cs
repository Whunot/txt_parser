using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SourceToMarkdownTests
{
    /// <summary>
    /// Утилита для чтения embedded ресурсов сборки
    /// </summary>
    class AssemblyResourceLoader
    {
        /// <summary>
        /// Возвращает поток ресурса, если таковой есть в сборке
        /// </summary>
        /// <param name="resourceNameContains">Строка, по содержанию которой в имени ищется ресурс</param>
        public static Stream? OpenResourceStream(string resourceNameContains)
        {
            var assembly = Assembly.GetEntryAssembly();
            var resourceFullName = assembly?.GetManifestResourceNames()
                .FirstOrDefault(_ => _.ToLower()
                    .Contains(resourceNameContains.ToLower()));

            if (!string.IsNullOrWhiteSpace(resourceFullName))
            {
                return assembly?.GetManifestResourceStream(resourceFullName);
            }

            return null;
        }
    }
}
