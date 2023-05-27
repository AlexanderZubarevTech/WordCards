namespace WordCards.Business.LanguageWords
{
    public sealed class LibraryFileInfo
    {
        public LibraryFileInfo()
        {            
        }

        public int WordsCount { get; set; }

        public int NewWordsCount { get; set; }

        public int WordsWithoutTranslation { get; set; }

        public int DuplicateWordCount { get; set; }
    }
}
