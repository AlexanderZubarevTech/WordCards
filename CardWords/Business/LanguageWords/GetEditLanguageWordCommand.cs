using WordCards.Business.Languages;
using WordCards.Configurations;
using WordCards.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WordCards.Business.LanguageWords
{
    public sealed class GetEditLanguageWordsCommand : EntityCommand, IGetEditLanguageWordCommand
    {        
        public EditLanguageWord Execute(int? id)
        {
            EditLanguageWord result;

            using (var db = new LanguageWordContext())
            {
                var word = id != null && id > 0 ? db.LanguageWords.First(x => x.Id == id) : null;

                var languageIds = new int[] { AppConfiguration.Instance.CurrentLanguage, AppConfiguration.Instance.CurrentTranslationLanguage };

                var languages = db.Languages.Where(x => languageIds.Contains(x.Id)).ToDictionary(x => x.Id);

                result = GetEditWord(word, languages);
            }

            return result;
        }

        private EditLanguageWord GetEditWord(LanguageWord? word, Dictionary<int, Language> languages)
        {
            var language = languages[AppConfiguration.Instance.CurrentLanguage];
            var translationLanguage = languages[AppConfiguration.Instance.CurrentTranslationLanguage];

            if (word == null)
            {
                return new EditLanguageWord(language, translationLanguage);
            }

            return new EditLanguageWord(word, language, translationLanguage);
        }
    }
}
