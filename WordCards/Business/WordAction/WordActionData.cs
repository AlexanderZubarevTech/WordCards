using WordCards.Business.LanguageWords;
using WordCards.Business.WordActivities;
using WordCards.Core.Entities;
using WordCards.Core.Helpers;
using System;

namespace WordCards.Business.WordAction
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

        private WordActionData(int id, string wordName, string transcription, string correctTranslation) 
            : this(id, wordName, transcription, correctTranslation, string.Empty, string.Empty, true, Side.None)
        {
        }

        private WordActionData(int id, string wordName, string transcription, string correctTranslation,
            string wrongWordName, string wrongTranslation, Side correctSide)
            : this(id, wordName, transcription, correctTranslation, wrongWordName, wrongTranslation, false, correctSide)
        {
        }

        private WordActionData(int id, string wordName, string transcription, string correctTranslation,
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
            Date = TimeHelper.GetCurrentDate();

            if(IsNewWord)
            {
                Result = WordActivityType.NewWord;

                return WordActivityType.NewWord;
            }

            if(side == CorrectSide)
            {
                Result = WordActivityType.CorrectAnswer;

                return WordActivityType.CorrectAnswer;
            }

            Result = WordActivityType.WrongAnswer;

            return WordActivityType.WrongAnswer;            
        }

        public string GetTranslationBySide(Side side)
        {
            return side == CorrectSide ? CorrectTranslation : WrongTranslation;
        }
    }
}
