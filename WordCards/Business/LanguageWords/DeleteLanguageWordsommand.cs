using System.Linq;
using WordCards.Business.WordAction;
using WordCards.Core.Commands;

namespace WordCards.Business.LanguageWords
{
    public sealed class DeleteLanguageWordCommand : EntityCommand, IDeleteLanguageWordCommand
    {
        public void Execute(int id)
        {
            using var db = new WordActionContext();

            var item = db.LanguageWords.First(x => x.Id == id);

            var errorActivities = db.ErrorWordActivities.Where(x => x.LanguageWordId == id).ToList();
            var activities = db.WordActivities.Where(x => x.LanguageWordId == id).ToList();

            if (errorActivities.Count > 0)
            {
                db.RemoveRange(errorActivities);
            }

            if (activities.Count > 0)
            {
                db.RemoveRange(activities);
            }

            db.Remove(item);

            db.SaveChanges();
        }
    }
}
