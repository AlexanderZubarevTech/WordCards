using CardWords.Configurations;
using CardWords.Core.Commands;
using CardWords.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using ValidationResult = CardWords.Core.Validations.ValidationResult;

namespace CardWords.Business.LanguageWords
{
    public sealed class CheckLibraryFileCommand : EntityCommand, ICheckLibraryFileCommand
    {
        private const string tableNodeName = "table";
        private const string rowNodeName = "row";
        private const string cellNodeName = "cell";
        private const string dataNodeName = "data";

        private ProgressBar progress;
        private Dispatcher mainDispatcher;

        public async Task<LibraryFileInfo> Execute(string fullFileName, Dispatcher mainDispatcher, ProgressBar progress)
        {
            this.progress = progress;
            this.mainDispatcher = mainDispatcher;

            var file = new FileInfo(fullFileName);

            var doc = new XmlDocument();
            doc.Load(file.OpenText());

            var tableNode = GetTableNodeRecursive(doc);

            if(tableNode == null)
            {
                ValidationResult.ThrowError($"Таблица не найдена. Тег {tableNode} не найден.");                
            }

            var wordCount = GetRowCount(tableNode);

            progress.Maximum = wordCount;

            var newVords = await CheckWords(tableNode);

            return new LibraryFileInfo(wordCount, newVords);
        }

        private XmlNode? GetTableNodeRecursive(XmlNode node)
        {
            if(node == null)
            {
                return null;
            }

            if(IsNode(node, tableNodeName))
            {
                return node;
            }

            foreach(XmlNode childNode in node.ChildNodes)
            {
                var result = GetTableNodeRecursive(childNode);

                if(result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static bool IsNode(XmlNode node, string name)
        {
            return node.Name.ToLower() == name;
        }

        private static int GetRowCount(XmlNode tableNode)
        {
            var count = 0;

            foreach(XmlNode childNode in tableNode.ChildNodes)
            {
                if(!IsNode(childNode, rowNodeName) 
                    || childNode.FirstChild == null 
                    || childNode.FirstChild.FirstChild == null
                    || childNode.FirstChild.FirstChild.FirstChild == null
                    || childNode.FirstChild.FirstChild.FirstChild.Value.IsNullOrEmptyOrWhiteSpace())
                {
                    continue;
                }
                
                count++;                
            }

            return count;
        }

        private async Task ChangeProgress()
        {
            await Task.Run(() => mainDispatcher.Invoke(() => progress.Value++));
        }

        private async Task<int> CheckWords(XmlNode tableNode)
        {
            int newWordsCount = 0;

            using (var db = new LanguageWordContext())
            {
                var wordList = new List<string>(200);                

                foreach(XmlNode rowNode in tableNode.ChildNodes)
                {
                    if(wordList.Count == 200)
                    {
                        newWordsCount += GetNewWordCountByList(db.LanguageWords, wordList);

                        wordList.Clear();                                
                    }

                    if(!IsNode(rowNode, rowNodeName))
                    {
                        continue;
                    }

                    if(rowNode.FirstChild == null 
                        || rowNode.FirstChild.FirstChild == null 
                        || rowNode.FirstChild.FirstChild.FirstChild == null)
                    {
                        continue;
                    }

                    var wordName = rowNode.FirstChild.FirstChild.FirstChild.Value;

                    if(!wordName.IsNullOrEmptyOrWhiteSpace())
                    {
                        wordList.Add(wordName.Trim().ToLower());

                        await ChangeProgress();
                    }
                }

                if(wordList.Count > 0)
                {
                    newWordsCount += GetNewWordCountByList(db.LanguageWords, wordList);
                }
            }

            return newWordsCount;
        }

        private static int GetNewWordCountByList(DbSet<LanguageWord> languageWords, List<string> list)
        {
            var existCount = languageWords.Where(x =>
                            x.LanguageId == AppConfiguration.Instance.CurrentLanguage &&
                            x.TranslationLanguageId == AppConfiguration.Instance.CurrentTranslationLanguage &&
                            list.Contains(x.LanguageWordName))
                            .Count();

            return list.Count - existCount;
        }
    }
}
