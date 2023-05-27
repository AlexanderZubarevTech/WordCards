using WordCards.Core.Commands;
using System.Linq;

namespace WordCards.Business.Languages
{
    public sealed class GetLanguageCommand : EntityCommand, IGetLanguageCommand
    {
        public Language Execute(int id)
        {
            Language result;

            using (var db = new LanguageContext())
            {
                result = db.Languages.First(x => x.Id == id);
            }

            return result;
        }
    }
}
