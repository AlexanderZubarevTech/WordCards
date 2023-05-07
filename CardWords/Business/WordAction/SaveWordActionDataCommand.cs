using CardWords.Business.WordActivities;
using CardWords.Configurations;
using CardWords.Core.Commands;
using System.Collections.Generic;

namespace CardWords.Business.WordAction
{
    public sealed class SaveWordActionDataCommand : EntityCommand, ISaveWordActionDataCommand
    {
        private readonly AppConfiguration configuration = AppConfiguration.GetInstance();

        public bool Execute(List<WordActionData> data)
        {
            using (var db = new WordActionContext())
            {
                foreach (var item in data)
                {
                    var activity = GetActivity(item);

                    db.WordActivities.Add(activity);
                }

                db.SaveChanges();
            }

            return true;
        }

        private WordActivity GetActivity(WordActionData item)
        {
            return new WordActivity(item.Date, item.Id, configuration.CurrentLanguage, item.Result);
        }
    }
}
