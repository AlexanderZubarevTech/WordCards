using CardWords.Business.LanguageWords;
using CardWords.Configurations;
using CardWords.Core.Commands;
using CardWords.Core.Ids;
using CardWords.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardWords.Business.WordAction
{
    public sealed class GetWordActionDataCommand : EntityCommand, IGetWordActionDataCommand
    {
        private static readonly Dictionary<Id, string> getCountNewWordsSqlDictionary = new Dictionary<Id, string>();
        private static readonly Dictionary<Id, string> getCountKnownWordsSqlDictionary = new Dictionary<Id, string>();        

        private Random random;
        private readonly AppConfiguration configuration = AppConfiguration.GetInstance();

        public List<WordActionData> Execute(int count)
        {
            var seed = (int)Math.Round(DateTime.Now.TimeOfDay.TotalSeconds);

            random = new Random(seed);

            List<WordActionData> result;

            using (var db = new WordActionContext())
            {
                result = GetData(db, count);
            }

            return result;
        }

        private List<WordActionData> GetData(WordActionContext db, int count)
        {
            var allNewWordsCount = GetCountAllNewWords(db);
            var allKnownWordsCount = GetCountAllKnownWords(db);

            var newWordsCount = CalculateNewWordsCount(count, allNewWordsCount, allKnownWordsCount);
            var knownWordsCount = CalculateKnownWordsCount(count, allKnownWordsCount, newWordsCount);

            var newWords = LoadNewWords(db, newWordsCount, allNewWordsCount);
            var knownWords = LoadKnownWords(db, knownWordsCount, allKnownWordsCount);

            var result = new List<WordActionData>();

            newWords.ForEach(x => WordActionData.Create(x).AddTo(result));

            for (int i = 0; i < knownWords.Length; i++)
            {
                int wrongWordIndex;

                while (true)
                {
                    wrongWordIndex  = GetRandom(0, knownWords.Length - 1);

                    if(wrongWordIndex != i || knownWords.Length == 1)
                    {
                        break;
                    }
                }

                var side = (WordActionData.Side)GetRandom((int)WordActionData.Side.Left, (int)WordActionData.Side.Right);

                WordActionData.Create(knownWords[i], knownWords[wrongWordIndex], side)
                    .AddTo(result);
            }

            return result;
        }

        private static string GetSqlCountNewWords(Id languageId)
        {
            if(getCountNewWordsSqlDictionary.ContainsKey(languageId))
            {
                return getCountNewWordsSqlDictionary[languageId];
            }

            var sql = $"SELECT count(*) as NewWordsCount FROM [language_words] as [lw] " +
                $"LEFT JOIN (" +
                $"      SELECT LanguageWordId, count(*) as count FROM [word_activities] as [wa] " +
                $"      WHERE wa.LanguageId = {languageId} " +
                $"      GROUP BY wa.LanguageWordId " +
                $") as t ON lw.Id = t.LanguageWordId " +
                $"WHERE lw.LanguageId = {languageId} AND t.count IS NULL";

            getCountNewWordsSqlDictionary.Add(languageId, sql);

            return sql;
        }

        private static string GetSqlCountKnownWords(Id languageId)
        {
            if (getCountKnownWordsSqlDictionary.ContainsKey(languageId))
            {
                return getCountKnownWordsSqlDictionary[languageId];
            }

            var sql = $"SELECT count(*) as NewWordsCount FROM [language_words] as [lw] " +
                $"LEFT JOIN (" +
                $"      SELECT LanguageWordId, count(*) as count FROM [word_activities] as [wa] " +
                $"      WHERE wa.LanguageId = {languageId} " +
                $"      GROUP BY wa.LanguageWordId " +
                $") as t ON lw.Id = t.LanguageWordId " +
                $"WHERE lw.LanguageId = {languageId} AND t.count IS NOT NULL";

            getCountKnownWordsSqlDictionary.Add(languageId, sql);

            return sql;
        }

        private int GetCountAllNewWords(WordActionContext db)
        {
            return db.Database.SqlQueryRaw<int>(GetSqlCountNewWords(configuration.CurrentLanguage)).ToList().FirstOrDefault();
        }

        private int GetCountAllKnownWords(WordActionContext db)
        {
            return db.Database.SqlQueryRaw<int>(GetSqlCountKnownWords(configuration.CurrentLanguage)).ToList().FirstOrDefault();
        }

        private int CalculateNewWordsCount(int count, int allNewWords, int allKnownWords)
        {
            if(allKnownWords == 0)
            {
                return allNewWords < count ? allNewWords : count;
            }

            if(allNewWords + allKnownWords < count) 
            {
                return allNewWords;
            }

            if(allKnownWords < count)
            {
                return count - allKnownWords;
            }

            // 30% вероятность отклонения запроса новых слов
            if(GetRandom(0, 10) < 4)
            {
                return 0;
            }

            if(count < 21)
            {
                return (int)Math.Round(count * 0.25);
            }

            if(count > 99)
            {
                return 10;
            }
            
            return (int)Math.Round(count * 0.1);
        }

        private static int CalculateKnownWordsCount(int count, int allKnownWords, int newWordsCount)
        {
            if(newWordsCount == 0 || allKnownWords < count || newWordsCount + allKnownWords < count)
            {
                return allKnownWords;
            }

            return count - newWordsCount;
        }

        private LanguageWord[] LoadKnownWords(WordActionContext db, int count, int allKnownWords)
        {
            if (count == 0)
            {
                return Array.Empty<LanguageWord>();
            }

            var offset = allKnownWords > count ? GetRandom(0, allKnownWords - count) : 0;            

            var sql = $"SELECT [lw].* FROM [language_words] as [lw] " +
                $"LEFT JOIN (" +
                $"SELECT LanguageWordId, count(*) as count FROM [word_activities] as [wa] " +
                $"WHERE wa.LanguageId = {configuration.CurrentLanguage} GROUP BY wa.LanguageWordId" +
                $") as t ON lw.Id = t.LanguageWordId " +
                $"WHERE lw.LanguageId = {configuration.CurrentLanguage} " +
                $"AND t.count IS NOT NULL " +
                $"ORDER BY t.count " +
                $"LIMIT {count} OFFSET {offset}";

            return db.Database.SqlQueryRaw<LanguageWord>(sql).ToArray();
        }

        private List<LanguageWord> LoadNewWords(WordActionContext db, int count, int allNewWords)
        {
            var idArr = LoadNewWordIds(db, count, allNewWords);

            var ids = SelectNewWordIds(idArr, count);

            return db.LanguageWords.Where(x => ids.Contains(x.Id)).ToList();
        }

        private Id[] LoadNewWordIds(WordActionContext db, int count, int allNewWords)
        {
            if(count == 0)
            {
                return Array.Empty<Id>();
            }

            var offset = allNewWords > count ? GetRandom(0, allNewWords - count) : 0;

            var sql = $"SELECT Id FROM [language_words] as [lw] " +
                $"WHERE [lw].LanguageId = {configuration.CurrentLanguage} " +
                $"LIMIT {count} OFFSET {offset}";

            return db.Database.SqlQueryRaw<int>(sql).Cast<Id>().ToArray();
        }

        private List<Id> SelectNewWordIds(Id[] ids, int count)
        {
            var result = new List<Id>();

            while (true)
            {
                var index = GetRandom(0, ids.Length - 1);

                var value = ids[index];

                if (result.Contains(value))
                {
                    continue;
                }

                result.Add(value);

                if(result.Count == count)
                {
                    break;
                }
            }

            return result;
        }

        private int GetRandom(int min, int max)
        {
            return random.Next(min, max + 1);
        }
    }
}
