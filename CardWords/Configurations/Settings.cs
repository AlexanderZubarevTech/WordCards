using CardWords.Business.Languages;
using CardWords.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CardWords.Configurations
{
    public sealed class Settings : Entity
    {
        public Settings(AppConfiguration configuration, Dictionary<int, Language> languages) 
        {
            CurrentLanguage = languages[configuration.CurrentLanguage];
            TranslationLanguage = languages[configuration.CurrentTranslationLanguage];
            WordCardHasTimer = configuration.WordCardHasTimer;
            WordCardTimerDurationInSeconds = configuration.WordCardTimerDurationInSeconds;
            Languages = languages.Values.ToList();
        }

        public Language CurrentLanguage { get; set; }

        public Language TranslationLanguage { get; set; }
        
        public bool WordCardHasTimer { get; set; }
        
        public int WordCardTimerDurationInSeconds { get; set; }

        public List<Language> Languages { get; set; }
    }
}
