﻿using CardWords.Business.LanguageWords;
using CardWords.Core.Commands;
using System.Linq;

namespace CardWords.Business.WordAction
{
    public sealed class DeleteLanguageWordCommand : EntityCommand, IDeleteLanguageWordCommand
    {
        public void Execute(int id)
        {
            using var db = new WordActionContext();

            var item = db.LanguageWords.First(x => x.Id == id);

            var errorActivities = db.ErrorWordActivities.Where(x => x.LanguageWordId == id).ToList();
            var activities = db.WordActivities.Where(x => x.LanguageWordId == id).ToList();            

            if(errorActivities.Count > 0)
            {
                db.RemoveRange(errorActivities);
            }

            if(activities.Count > 0)
            {
                db.RemoveRange(activities);
            }

            db.Remove(item);

            db.SaveChanges();
        }
    }
}
