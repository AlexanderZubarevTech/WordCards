using WordCards.Configurations;
using WordCards.Core.Commands;
using WordCards.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace WordCards.Business.LanguageWords
{
    public sealed class GetLanguageWordsCommand : EntityCommand, IGetLanguageWordsCommand
    {
        public ObservableCollection<LanguageWordView> Execute(string name, bool withoutTranscription, WordStatus status)
        {
            ObservableCollection<LanguageWordView> result;

            using (var db = new LanguageWordContext())
            {
                var queryInn = from wa in db.WordActivities
                               join lw in db.LanguageWords on wa.LanguageWordId equals lw.Id
                               where lw.LanguageId == AppConfiguration.Instance.CurrentLanguage
                               && lw.TranslationLanguageId == AppConfiguration.Instance.CurrentTranslationLanguage
                               group wa by wa.LanguageWordId into g
                               select new
                               {
                                    LanguageWordId = g.Key,
                                    Count = g.Count()
                               };

                var query = from lw in db.LanguageWords
                            join t in queryInn on lw.Id equals t.LanguageWordId into inn
                            from sub in inn.DefaultIfEmpty()
                            where lw.LanguageId == AppConfiguration.Instance.CurrentLanguage
                            && lw.TranslationLanguageId == AppConfiguration.Instance.CurrentTranslationLanguage
                            select new LanguageWordView
                            {
                                Id = lw.Id,
                                LanguageWordName = lw.LanguageWordName,
                                Transcription = lw.Transcription,
                                Translation = lw.Translation,
                                IsNewWord = sub.Count == null
                            };


                if (!name.IsNullOrEmptyOrWhiteSpace())
                {
                    var words = name.ToLower().Split(',').Select(x => x.Trim());

                    query = query.Where(x => words.Contains(x.LanguageWordName));
                }

                if (withoutTranscription)
                {
                    query = query.Where(x => x.Transcription == string.Empty);
                }

                if(status == WordStatus.NewWord)
                {
                    query = query.Where(x => x.IsNewWord);
                }
                else if(status == WordStatus.LearnedWord)
                {
                    query = query.Where(x => !x.IsNewWord);
                }

                result = new ObservableCollection<LanguageWordView>(query.OrderBy(x => x.LanguageWordName).ToList());
            }

            return result;
        }
    }
}
