using CardWords.Business.WordActivities;
using CardWords.Configurations;
using CardWords.Core.Commands;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CardWords.Business.WordAction
{
    public sealed class SaveWordActionDataCommand : EntityCommand, ISaveWordActionDataCommand
    {
        private readonly AppConfiguration configuration = AppConfiguration.GetInstance();

        public bool Execute(WordActionData[] data, WordActionInfo info)
        {
            using (var db = new WordActionContext())
            {
                db.WordActionInfos.Add(info);

                db.SaveChanges();

                var errorActivities = GetErrorActivity(db, data);

                foreach (var item in data)
                {
                    var activity = GetActivity(item, info.Id);

                    db.WordActivities.Add(activity);

                    UpdateErrorActivity(db.ErrorWordActivities, errorActivities, item, info.Id);                    
                }                

                db.SaveChanges();
            }

            return true;
        }

        private WordActivity GetActivity(WordActionData item, int infoId)
        {
            return new WordActivity(item.Date, item.Id, configuration.CurrentLanguage, item.Result, infoId);
        }

        private IReadOnlyDictionary<int, ErrorWordActivity> GetErrorActivity(WordActionContext db, WordActionData[] data)
        {
            var wordIds = data.Select(x => x.Id).ToList();

            return db.ErrorWordActivities
                .Where(x => wordIds.Contains(x.LanguageWordId))
                .ToDictionary(x => x.LanguageWordId, x => x);
        }

        private void UpdateErrorActivity(DbSet<ErrorWordActivity> db, IReadOnlyDictionary<int, ErrorWordActivity> errorActivities, WordActionData item, int infoId)
        {
            if (item.Result == WordActivityType.CorrectAnswer && errorActivities.ContainsKey(item.Id))
            {
                db.Remove(errorActivities[item.Id]);

                return;
            }

            if (item.Result == WordActivityType.WrongAnswer)
            {
                if (errorActivities.ContainsKey(item.Id))
                {
                    var errorActivity = errorActivities[item.Id];

                    errorActivity.InfoId = infoId;

                    db.Update(errorActivity);
                }
                else
                {
                    var errorActivity = new ErrorWordActivity(item.Id, configuration.CurrentLanguage, infoId);

                    db.Add(errorActivity);
                }
            }
        }
    }
}
