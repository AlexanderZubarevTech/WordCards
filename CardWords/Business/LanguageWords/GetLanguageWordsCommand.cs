using CardWords.Configurations;
using CardWords.Core.Commands;
using CardWords.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardWords.Business.LanguageWords
{
    public sealed class GetLanguageWordsCommand : EntityCommand, IGetLanguageWordsCommand
    {
        public ObservableCollection<LanguageWord> Execute(string name, bool withoutTranscription)
        {
            ObservableCollection<LanguageWord> result;

            using (var db = new LanguageWordContext())
            {
                var query = db.LanguageWords
                    .Where(x => x.LanguageId == AppConfiguration.Instance.CurrentLanguage 
                    && x.TranslationLanguageId == AppConfiguration.Instance.CurrentTranslationLanguage);

                if(!name.IsNullOrEmptyOrWhiteSpace())
                {
                    var words = name.ToLower().Split(',').Select(x => x.Trim());

                    query = query.Where(x => words.Contains(x.LanguageWordName));                    
                }

                if(withoutTranscription)
                {
                    query = query.Where(x => x.Transcription == string.Empty);
                }

                result = new ObservableCollection<LanguageWord>(query.OrderBy(x => x.LanguageWordName).ToList());
            }

            return result;
        }
    }
}
