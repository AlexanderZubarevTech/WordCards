using CardWords.Business.Languages;
using CardWords.Core.Entities;

namespace CardWords.Business.LanguageWords
{
    public sealed class EditLanguageWord : Entity
    {
        public EditLanguageWord(Language language, Language translationLanguage) 
        {
            LanguageId = language.Id;
            LanguageName = language.Name;
            TranslationLanguageId = translationLanguage.Id;
            TranslationLanguageName = translationLanguage.Name;

            LanguageWordName = string.Empty;
            Transcription = string.Empty;
            Translation = string.Empty;
        }

        public EditLanguageWord(LanguageWord word, Language language, Language translationLanguage) 
            : this(language, translationLanguage)
        {
            Id = word.Id;
            Timestamp = word.Timestamp;
            LanguageWordName = word.LanguageWordName;
            Transcription = word.Transcription;
            Translation = word.Translation;
        }
        

        public int LanguageId { get; set; }

        public string LanguageName { get; set; }

        public int TranslationLanguageId { get; set; }

        public string TranslationLanguageName { get; set; }

        public string LanguageWordName { get; set; }

        public string Transcription { get; set; }

        public string Translation { get; set; }
    }
}
