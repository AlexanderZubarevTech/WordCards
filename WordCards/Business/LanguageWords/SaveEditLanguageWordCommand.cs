using System.Linq;
using WordCards.Core.Commands;
using WordCards.Core.Helpers;
using WordCards.Core.Validations;
using WordCards.Extensions;

namespace WordCards.Business.LanguageWords
{
    public sealed class SaveEditLanguageWordsCommand : EntityCommand, ISaveEditLanguageWordCommand
    {
        public bool Execute(EditLanguageWord entity)
        {
            Validate(entity);

            using (var db = new LanguageWordContext())
            {
                var isEdit = entity.Id > 0;

                if (!isEdit)
                {
                    var existWord = db.LanguageWords.FirstOrDefault(x => x.LanguageWordName == entity.LanguageWordName);

                    if (existWord != null)
                    {
                        ValidationResult.ThrowError("Слово уже существует в библиотеке!");
                    }
                }

                var word = new LanguageWord(entity, TimeHelper.GetCurrentDate());

                if (isEdit)
                {
                    db.LanguageWords.Update(word);
                }
                else
                {
                    db.LanguageWords.Add(word);
                }

                db.SaveChanges();
            }

            return true;
        }

        private static void Validate(EditLanguageWord editWord)
        {
            var validationResult = new ValidationResult();

            if (editWord.LanguageWordName.IsNullOrEmptyOrWhiteSpace())
            {
                validationResult.AddRequired<EditLanguageWord, string>(x => x.LanguageWordName);
            }

            if (editWord.Translation.IsNullOrEmptyOrWhiteSpace())
            {
                validationResult.AddRequired<EditLanguageWord, string>(x => x.Translation);
            }

            validationResult.ThrowIfHasError();
        }
    }
}
