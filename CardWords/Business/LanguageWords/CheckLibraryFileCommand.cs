using WordCards.Configurations;
using WordCards.Core.Commands;
using WordCards.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using ValidationResult = WordCards.Core.Validations.ValidationResult;

namespace WordCards.Business.LanguageWords
{
    public sealed class CheckLibraryFileCommand : EntityCommand, ICheckLibraryFileCommand
    {
        private const string tableNodeName = "table";
        private const string rowNodeName = "row";        

        private ProgressBar progress;
        private Dispatcher mainDispatcher;

        public async Task<LibraryFileInfo> Execute(string fullFileName, Dispatcher mainDispatcher, ProgressBar progress)
        {
            this.progress = progress;
            this.mainDispatcher = mainDispatcher;

            var file = new FileInfo(fullFileName);

            var doc = new XmlDocument();

            try
            {
                doc.Load(file.OpenText());
            }
            catch (IOException ex)
            {
                ValidationResult.ThrowError($"Ошибка открытия файла: {ex.Message}");

                return null;
            }

            var tableNode = GetTableNodeRecursive(doc);

            if(tableNode == null)
            {
                ValidationResult.ThrowError($"Таблица не найдена. Тег {tableNode} не найден.");
            }

            var info = new LibraryFileInfo();

            var wordCount = GetRowCount(tableNode);

            progress.Maximum = wordCount;
            info.WordsCount = wordCount;

            await CheckWords(tableNode, info);

            return info;
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

        private async Task CheckWords(XmlNode tableNode, LibraryFileInfo info)
        {
            using (var db = new LanguageWordContext())
            {
                var wordList = new List<string>(200);

                var words = new HashSet<string>(info.WordsCount);

                foreach(XmlNode rowNode in tableNode.ChildNodes)
                {
                    if(wordList.Count == 200)
                    {
                        info.NewWordsCount += GetNewWordCountByList(db.LanguageWords, wordList);

                        wordList.Clear();                              
                    }

                    if(!IsNode(rowNode, rowNodeName))
                    {
                        continue;
                    }

                    var wordNode = rowNode.ChildNodes[0];

                    if (wordNode == null 
                        || wordNode.FirstChild == null
                        || wordNode.FirstChild.FirstChild == null 
                        || wordNode.FirstChild.FirstChild.Value.IsNullOrEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    await ChangeProgress();

                    var wordName = wordNode.FirstChild.FirstChild.Value.Trim().ToLower();

                    if (words.Contains(wordName))
                    {
                        info.DuplicateWordCount++;

                        continue;
                    }
                    else
                    {
                        words.Add(wordName);
                    }

                    var translationNode = rowNode.ChildNodes[1];

                    if (translationNode == null
                        || translationNode.FirstChild == null
                        || translationNode.FirstChild.FirstChild == null
                        || translationNode.FirstChild.FirstChild.Value.IsNullOrEmptyOrWhiteSpace())
                    {
                        info.WordsWithoutTranslation++;

                        continue;
                    }

                    wordList.Add(wordName);                    
                }

                if(wordList.Count > 0)
                {
                    info.NewWordsCount += GetNewWordCountByList(db.LanguageWords, wordList);
                }
            }            
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
