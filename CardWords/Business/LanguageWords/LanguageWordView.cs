using WordCards.Core.Entities;

namespace WordCards.Business.LanguageWords
{
    public sealed class LanguageWordView : Entity
    {
        public LanguageWordView() 
        {            
        }

        public string LanguageWordName { get; set; }

        public string Transcription { get; set; }

        public string Translation { get; set; }

        public bool IsNewWord { get; set; }
    }
}
