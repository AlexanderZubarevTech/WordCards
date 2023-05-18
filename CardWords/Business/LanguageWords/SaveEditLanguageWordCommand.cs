using CardWords.Core.Commands;
using CardWords.Core.Helpers;
using System.Linq;

namespace CardWords.Business.LanguageWords
{
    public sealed class SaveEditLanguageWordsCommand : EntityCommand, ISaveEditLanguageWordCommand
    {
        public bool Execute(EditLanguageWord editWord)
        {
            using (var db = new LanguageWordContext())
            {
                var isEdit = editWord.Id > 0;

                if (!isEdit)
                {
                    var existWord = db.LanguageWords.FirstOrDefault(x => x.LanguageWordName  == editWord.LanguageWordName);

                    if(existWord != null)
                    {
                        return false;
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
    }
}
