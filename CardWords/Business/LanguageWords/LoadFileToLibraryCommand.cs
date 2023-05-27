using WordCards.Configurations;
using WordCards.Core.Commands;
using WordCards.Core.Helpers;
using WordCards.Extensions;
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
    public sealed class LoadFileToLibraryCommand : EntityCommand, ILoadFileToLibraryCommand
    {
        private const string tableNodeName = "table";
        private const string rowNodeName = "row";
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

            var tableNode = FindNode(doc, tableNodeName);

            if(tableNode == null)
            {
                ValidationResult.ThrowError($"Таблица не найдена. Тег {tableNode} не найден.");                
            }

            var info = new LibraryFileInfo();

            var wordCount = GetRowCount(tableNode);

            progress.Maximum = wordCount;
            info.WordsCount = wordCount;

            await SaveWords(tableNode, info);

            return info;
        }

        private XmlNode? FindNode(XmlNode? node, string nodeName)
        {
            if(node == null)
            {
                return null;
            }

            if(IsNode(node, nodeName))
            {
                return node;
            }

            foreach(XmlNode childNode in node.ChildNodes)
            {
                var result = FindNode(childNode, nodeName);

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

            foreach (XmlNode childNode in tableNode.ChildNodes)
            {
                if (!IsNode(childNode, rowNodeName)
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

        private async Task SaveWords(XmlNode tableNode, LibraryFileInfo info)
        {
            using (var db = new LanguageWordContext())
            {
                var wordList = new List<LanguageWord>(200);

                var timestamp = TimeHelper.GetCurrentDate();

                var words = new HashSet<string>(info.WordsCount);

                foreach (XmlNode rowNode in tableNode.ChildNodes)
                {
                    if(wordList.Count == 200)
                    {
                        info.NewWordsCount += SaveNewWordCountByList(db, wordList);

                        wordList.Clear();
                    }

                    if(!IsNode(rowNode, rowNodeName))
                    {
                        continue;
                    }

                    var wordNode = rowNode.ChildNodes[0];
                    var translationNode = rowNode.ChildNodes[1];
                    var transcriptionNode = rowNode.ChildNodes[2];

                    var dataWordNode = FindNode(wordNode, dataNodeName);

                    if(dataWordNode == null 
                        || dataWordNode.FirstChild == null 
                        || dataWordNode.FirstChild.Value.IsNullOrEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    await ChangeProgress();

                    var wordName = dataWordNode.FirstChild.Value.Trim().ToLower();

                    if(words.Contains(wordName))
                    {
                        info.DuplicateWordCount++;

                        continue;
                    } 
                    else
                    {
                        words.Add(wordName);
                    }

                    var dataTranslationNode = FindNode(translationNode, dataNodeName);

                    if (dataTranslationNode == null 
                        || dataTranslationNode.FirstChild == null 
                        || dataTranslationNode.FirstChild.Value.IsNullOrEmptyOrWhiteSpace())
                    {
                        info.WordsWithoutTranslation++;

                        continue;
                    }
                    
                    var translationName = dataTranslationNode.FirstChild.Value.Trim().ToLower();

                    var transcriptionDataNode = FindNode(transcriptionNode, dataNodeName);

                    var transcription = transcriptionDataNode != null && transcriptionDataNode.FirstChild != null 
                        ? transcriptionDataNode.FirstChild.Value.Trim()
                        : string.Empty;

                    var word = new LanguageWord()
                    {
                        Timestamp = timestamp,
                        LanguageId = AppConfiguration.Instance.CurrentLanguage,
                        TranslationLanguageId = AppConfiguration.Instance.CurrentTranslationLanguage,
                        LanguageWordName = wordName,
                        Transcription = transcription,
                        Translation = translationName
                    };

                    wordList.Add(word);                    
                }

                if(wordList.Count > 0)
                {
                    info.NewWordsCount += SaveNewWordCountByList(db, wordList);
                }                
            }            
        }        

        private static int SaveNewWordCountByList(LanguageWordContext db, List<LanguageWord> list)
        {
            var names = list.Select(x => x.LanguageWordName);

            var exists = db.LanguageWords
                .Where(x => x.LanguageId == AppConfiguration.Instance.CurrentLanguage
                            && x.TranslationLanguageId == AppConfiguration.Instance.CurrentTranslationLanguage
                            && names.Contains(x.LanguageWordName))
                .Select(x => x.LanguageWordName)
                .ToList();

            var newCount = list.Count - exists.Count;

            if(newCount > 0)
            {
                var saveList = list.Where(x => !exists.Contains(x.LanguageWordName));

                db.LanguageWords.AddRange(saveList);

                db.SaveChanges();
            }

            return newCount;
        }
    }
}
