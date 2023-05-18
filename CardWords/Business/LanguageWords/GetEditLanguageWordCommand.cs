using CardWords.Business.Languages;
using CardWords.Configurations;
using CardWords.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardWords.Business.LanguageWords
{
    public sealed class GetEditLanguageWordsCommand : EntityCommand, IGetEditLanguageWordCommand
    {        
        private readonly AppConfiguration configuration = AppConfiguration.GetInstance();

        public EditLanguageWord Execute(int? id)
        {
            EditLanguageWord result;

            using (var db = new LanguageWordContext())
            {
                var word = id != null && id > 0 ? db.LanguageWords.First(x => x.Id == id) : null;

                var languageIds = new int[] {configuration.CurrentLanguage, configuration.CurrentTranslationLanguage };

                var languages = db.Languages.Where(x => languageIds.Contains(x.Id)).ToDictionary(x => x.Id);

                result = GetEditWord(word, languages);
            }

            return result;
        }

        private EditLanguageWord GetEditWord(LanguageWord? word, Dictionary<int, Language> languages)
        {
            var language = languages[configuration.CurrentLanguage];
            var translationLanguage = languages[configuration.CurrentTranslationLanguage];

            if (word == null)
            {
                return new EditLanguageWord(language, translationLanguage);
            }

            return new EditLanguageWord(word, language, translationLanguage);
        }
    }
}
