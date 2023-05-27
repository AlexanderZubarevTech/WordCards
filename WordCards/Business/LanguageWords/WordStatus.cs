using System.Collections.Generic;

namespace WordCards.Business.LanguageWords
{
    public sealed class WordStatus
    {
        public enum StatusEnum
        {
            Any,
            NewWord,
            LearnedWord
        }

        public static readonly WordStatus Any = new WordStatus(StatusEnum.Any, "Любой");
        public static readonly WordStatus NewWord = new WordStatus(StatusEnum.NewWord, "Новое слово");
        public static readonly WordStatus LearnedWord = new WordStatus(StatusEnum.LearnedWord, "Изученное слово");

        public static readonly IReadOnlyDictionary<StatusEnum, WordStatus> Items = new Dictionary<StatusEnum, WordStatus>
        {
            {StatusEnum.Any, Any},
            {StatusEnum.NewWord, NewWord},
            {StatusEnum.LearnedWord, LearnedWord},
        };

        private WordStatus(StatusEnum status, string name) 
        {            
            Status = status;
            Name = name;
        }

        public StatusEnum Status { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
