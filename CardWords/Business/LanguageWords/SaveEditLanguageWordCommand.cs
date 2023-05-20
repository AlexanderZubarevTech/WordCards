using CardWords.Core.Commands;
using CardWords.Core.Helpers;
using CardWords.Core.Validations;
using CardWords.Extensions;
using System.Linq;

namespace CardWords.Business.LanguageWords
{
    public sealed class SaveEditLanguageWordsCommand : EntityCommand, ISaveEditLanguageWordCommand
    {
        public bool Execute(EditLanguageWord editWord)
        {
            Validate(editWord);

            using (var db = new LanguageWordContext())
            {
                var isEdit = editWord.Id > 0;

                if (!isEdit)
                {
                    var existWord = db.LanguageWords.FirstOrDefault(x => x.LanguageWordName  == editWord.LanguageWordName);

                    if(existWord != null)
                    {
                        ValidationResult.ThrowError("Слово уже существует в библиотеке!");
                    }
                }

                var word = new LanguageWord(editWord, TimeHelper.GetCurrentDate());

                if(isEdit)
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

            if(editWord.LanguageWordName.IsNullOrEmptyOrWhiteSpace())
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
