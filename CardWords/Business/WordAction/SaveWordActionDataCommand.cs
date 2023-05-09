using CardWords.Business.WordActivities;
using CardWords.Configurations;
using CardWords.Core.Commands;

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

                foreach (var item in data)
                {
                    var activity = GetActivity(item, info.Id);

                    db.WordActivities.Add(activity);
                }                

                db.SaveChanges();
            }

            return true;
        }

        private WordActivity GetActivity(WordActionData item, int infoId)
        {
            return new WordActivity(item.Date, item.Id, configuration.CurrentLanguage, item.Result, infoId);
        }
    }
}
