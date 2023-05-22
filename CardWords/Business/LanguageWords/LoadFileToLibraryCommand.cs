using CardWords.Configurations;
using CardWords.Core.Commands;
using CardWords.Core.Helpers;
using CardWords.Extensions;
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

            var wordCount = GetRowCount(tableNode);

            progress.Maximum = wordCount;

            var newVords = await SaveWords(tableNode);

            return new LibraryFileInfo(wordCount, newVords);
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

            foreach(XmlNode childNode in tableNode.ChildNodes)
            {
                if(IsNode(childNode, rowNodeName) && !childNode.ChildNodes[0].ChildNodes[0].InnerText.IsNullOrEmptyOrWhiteSpace())
                {
                    count++;
                }
            }

            return count;
        }

        private async Task ChangeProgress()
        {
            await Task.Run(() => mainDispatcher.Invoke(() => progress.Value++));
        }

        private async Task<int> SaveWords(XmlNode tableNode)
        {
            int newWordsCount = 0;

            using (var db = new LanguageWordContext())
            {
                var wordList = new List<LanguageWord>(200);

                var timestamp = TimeHelper.GetCurrentDate();

                foreach (XmlNode rowNode in tableNode.ChildNodes)
                {
                    if(wordList.Count == 200)
                    {
                        newWordsCount += SaveNewWordCountByList(db, wordList);

                        wordList.Clear();                                
                    }

                    if(!IsNode(rowNode, rowNodeName))
                    {
                        continue;
                    }

                    var wordNode = rowNode.ChildNodes[0];
                    var transcriptionNode = rowNode.ChildNodes[1];
                    var translationNode = rowNode.ChildNodes[2];

                    var dataWordNode = FindNode(wordNode, dataNodeName);

                    if(dataWordNode == null 
                        || dataWordNode.FirstChild == null 
                        || dataWordNode.FirstChild.Value.IsNullOrEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    var dataTranslationNode = FindNode(translationNode, dataNodeName);

                    if (dataTranslationNode == null 
                        || dataTranslationNode.FirstChild == null 
                        || dataTranslationNode.FirstChild.Value.IsNullOrEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    var wordName = dataWordNode.FirstChild.Value;
                    var translationName = dataTranslationNode.FirstChild.Value;

                    var transcriptionDataNode = FindNode(transcriptionNode, dataNodeName);

                    var transcription = transcriptionDataNode != null && transcriptionDataNode.FirstChild != null 
                        ? transcriptionDataNode.FirstChild.Value 
                        : string.Empty;

                    if (!wordName.IsNullOrEmptyOrWhiteSpace())
                    {
                        var word = new LanguageWord()
                        {
                            Timestamp = timestamp,
                            LanguageId = AppConfiguration.Instance.CurrentLanguage,
                            TranslationLanguageId = AppConfiguration.Instance.CurrentTranslationLanguage,
                            LanguageWordName = wordName.Trim().ToLower(),
                            Transcription = transcription.Trim(),
                            Translation = translationName.Trim().ToLower()
                        };

                        wordList.Add(word);

                        await ChangeProgress();
                    }
                }

                if(wordList.Count > 0)
                {
                    newWordsCount += SaveNewWordCountByList(db, wordList);
                }
            }

            return newWordsCount;
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
