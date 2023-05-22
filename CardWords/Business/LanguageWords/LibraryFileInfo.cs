namespace CardWords.Business.LanguageWords
{
    public sealed class LibraryFileInfo
    {
        public LibraryFileInfo(int wordsCount, int newWordsCount)
        {
            WordsCount = wordsCount;
            NewWordsCount = newWordsCount;
        }

        public int WordsCount { get; }

        public int NewWordsCount { get; }
    }
}
