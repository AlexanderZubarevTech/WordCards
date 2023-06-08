using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using WordCards.Configurations;
using WordCards.Core.Commands;
using WordCards.Core.Helpers;
using WordCards.Core.Validations;

namespace WordCards.Business.Languages
{
    public sealed class DeleteLanguageCommand : EntityCommand, IDeleteLanguageCommand
    {
        public void Execute(int id)
        {
            using (var db = new LanguageContext())
            {
                var count = db.Languages.Count();

                if (count <= 2)
                {
                    ValidationResult.ThrowError("Не получилось удалить. Должно сущетвовать минимум 2 языка.");
                }

                var language = db.Languages.First(x => x.Id == id);

                DeleteReferences(db.Database, id);

                UpdateSettings(db.Languages, id);

                db.Languages.Remove(language);

                db.SaveChanges();
            }
        }

        private static void DeleteReferences(DatabaseFacade db, int id)
        {
            DeleteWordActivityErrors(db, id);
            DeleteWordActivities(db, id);
            DeleteWordActions(db, id);
            DeleteWords(db, id);
        }

        private static void DeleteWordActivityErrors(DatabaseFacade db, int id)
        {
            DeleteWordActivityErrorsByAction(db, id);
            DeleteWordActivityErrorsByWord(db, id);
        }

        private static void DeleteWordActivityErrorsByAction(DatabaseFacade db, int id)
        {
            var sql = $"DELETE FROM [word_activity_errors] " +
                      $"  WHERE InfoId IN( " +
                      $"  SELECT Id FROM [word_actions] as [wa]  " +
                      $"  WHERE wa.LanguageId = {id} OR wa.TranslationLanguageId = {id} " +
                      $"  )";

            db.ExecuteSqlRaw(sql);
        }

        private static void DeleteWordActivityErrorsByWord(DatabaseFacade db, int id)
        {
            var sql = $"DELETE FROM [word_activity_errors] " +
                      $"   WHERE LanguageWordId IN( " +
                      $"   SELECT Id FROM[language_words] as [lw] " +
                      $"   WHERE lw.LanguageId = {id} OR lw.TranslationLanguageId = {id} " +
                      $"  )";

            db.ExecuteSqlRaw(sql);
        }

        private static void DeleteWordActivities(DatabaseFacade db, int id)
        {
            DeleteWordActivitiesByAction(db, id);
            DeleteWordActivitiesByWord(db, id);
        }

        private static void DeleteWordActivitiesByAction(DatabaseFacade db, int id)
        {
            var sql = $"DELETE FROM [word_activities] " +
                      $"  WHERE InfoId IN( " +
                      $"  SELECT Id FROM [word_actions] as [wa]  " +
                      $"  WHERE wa.LanguageId = {id} OR wa.TranslationLanguageId = {id} " +
                      $"  )";

            db.ExecuteSqlRaw(sql);
        }

        private static void DeleteWordActivitiesByWord(DatabaseFacade db, int id)
        {
            var sql = $"DELETE FROM [word_activities] " +
                      $"   WHERE LanguageWordId IN( " +
                      $"   SELECT Id FROM[language_words] as [lw] " +
                      $"   WHERE lw.LanguageId = {id} OR lw.TranslationLanguageId = {id} " +
                      $"  )";

            db.ExecuteSqlRaw(sql);
        }

        private static void DeleteWordActions(DatabaseFacade db, int id)
        {
            var sql = $"DELETE FROM [word_actions] " +
                      $"WHERE LanguageId = {id} OR TranslationLanguageId = {id} ";

            db.ExecuteSqlRaw(sql);
        }

        private static void DeleteWords(DatabaseFacade db, int id)
        {
            var sql = $"DELETE FROM [language_words] " +
                      $"WHERE LanguageId = {id} OR TranslationLanguageId = {id} ";

            db.ExecuteSqlRaw(sql);
        }

        private static void UpdateSettings(DbSet<Language> languages, int id)
        {
            var ids = new int[]
            {
                AppConfiguration.Instance.CurrentLanguage,
                AppConfiguration.Instance.CurrentTranslationLanguage
            };

            if (!ids.Contains(id))
            {
                return;
            }

            var anotherLanguage = languages.First(x => !ids.Contains(x.Id));

            var settings = CommandHelper.GetCommand<IGetSettingsCommand>().Execute();

            if (settings.CurrentLanguage.Id == id)
            {
                settings.CurrentLanguage = anotherLanguage;
            }

            if (settings.TranslationLanguage.Id == id)
            {
                settings.TranslationLanguage = anotherLanguage;
            }

            CommandHelper.GetCommand<ISaveSettingsCommand>().Execute(settings);
        }
    }
}
