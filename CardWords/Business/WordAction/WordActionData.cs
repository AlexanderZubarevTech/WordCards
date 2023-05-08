using CardWords.Business.LanguageWords;
using CardWords.Business.WordActivities;
using CardWords.Core.Entities;
using CardWords.Core.Ids;
using System;

namespace CardWords.Business.WordAction
{
    public sealed class WordActionData : Entity
    {
        public enum Side : int
        {
            None = 0,
            Left = 1,
            Right = 2
        }

        public static WordActionData Create(LanguageWord newWord)
        {
            return new WordActionData(newWord.Id, newWord.LanguageWordName, newWord.Transcription, newWord.Translation);
        }

        public static WordActionData Create(LanguageWord knownWord, LanguageWord wrongWord, Side correctSide)
        {
            return new WordActionData(knownWord.Id, knownWord.LanguageWordName, knownWord.Transcription, knownWord.Translation, 
                wrongWord.LanguageWordName, wrongWord.Translation, correctSide);
        }

        private WordActionData(Id id, string wordName, string transcription, string correctTranslation) 
            : this(id, wordName, transcription, correctTranslation, string.Empty, string.Empty, true, Side.None)
        {
        }

        private WordActionData(Id id, string wordName, string transcription, string correctTranslation,
            string wrongWordName, string wrongTranslation, Side correctSide)
            : this(id, wordName, transcription, correctTranslation, wrongWordName, wrongTranslation, false, correctSide)
        {
        }

        private WordActionData(Id id, string wordName, string transcription, string correctTranslation,
            string wrongWordName, string wrongTranslation, bool isNewWord, Side correctSide)
            : base(id)
        {            
            WordName = wordName;
            Transcription = transcription;
            CorrectTranslation = correctTranslation;
            WrongWordName = wrongWordName;
            WrongTranslation = wrongTranslation;
            IsNewWord = isNewWord;
            CorrectSide = correctSide;
        }

        public string WordName { get; }

        public string Transcription { get; }

        public string CorrectTranslation { get; }

        public string WrongWordName { get; }

        public string WrongTranslation { get; }

        public bool IsNewWord { get; }

        public Side CorrectSide { get; }

        public WordActivityType Result { get; private set; }

        public DateTime Date { get; private set; }

        public WordActivityType Check(Side side = Side.None)
        {
            Date = DateTime.Now;

            if(IsNewWord)
            {
                Result = WordActivityType.NewWord;

                return WordActivityType.NewWord;
            }

            if(side == CorrectSide)
            {
                Result = WordActivityType.TrueAnswer;

                return WordActivityType.TrueAnswer;
            }

            Result = WordActivityType.FalseAnswer;

            return WordActivityType.FalseAnswer;            
        }

        public string GetTranslationBySide(Side side)
        {
            return side == CorrectSide ? CorrectTranslation : WrongTranslation;
        }
    }
}
