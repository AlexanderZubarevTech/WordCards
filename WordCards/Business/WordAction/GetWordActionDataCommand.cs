using WordCards.Business.LanguageWords;
using WordCards.Configurations;
using WordCards.Core.Commands;
using WordCards.Core.Helpers;
using WordCards.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WordCards.Business.WordAction
{
    public sealed class GetWordActionDataCommand : EntityCommand, IGetWordActionDataCommand
    {
        private readonly struct LanguagePair
        {
            public LanguagePair(int languageId, int translationLanguageId)
            {
                LanguageId = languageId;
                TranslationLanguageId = translationLanguageId;
            }

            public int LanguageId { get; }

            public int TranslationLanguageId { get; }

            public static bool operator ==(LanguagePair left, LanguagePair right)
            {
                return left.LanguageId == right.LanguageId && left.TranslationLanguageId == right.TranslationLanguageId;
            }

            public static bool operator !=(LanguagePair left, LanguagePair right)
            {
                return !(left == right);
            }            

            public override bool Equals(object? obj)
            {
                if(obj == null || obj is not LanguagePair)
                {
                    return false;
                }

                var pair = (LanguagePair) obj;

                return this == pair;
            }

            public override int GetHashCode()
            {
                return LanguageId ^ TranslationLanguageId;
            }
        }

        private static readonly Dictionary<LanguagePair, string> getCountNewWordsSqlDictionary = new Dictionary<LanguagePair, string>();
        private static readonly Dictionary<LanguagePair, string> getCountKnownWordsSqlDictionary = new Dictionary<LanguagePair, string>();        

        private Random random;        
        private LanguagePair currentLanguage;

        public WordActionData[] Execute(int count)
        {
            var seed = (int)Math.Round(DateTime.Now.TimeOfDay.TotalSeconds);

            random = new Random(seed);
            currentLanguage = new LanguagePair(AppConfiguration.Instance.CurrentLanguage, AppConfiguration.Instance.CurrentTranslationLanguage);

            List<WordActionData> result;

            using (var db = new WordActionContext())
            {
                result = GetData(db, count);
            }

            return result.ToArray();
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

            AddData(newWords, knownWords, count, result);

            return result;
        }

        private void AddData(List<LanguageWord> newWords, LanguageWord[] knownWords, int count, List<WordActionData> result)
        {
            var decades = Math.Round(count / 10d, 0, MidpointRounding.ToPositiveInfinity);

            var newWordsCountOnDecade = (int) Math.Round(newWords.Count / decades, 0, MidpointRounding.ToPositiveInfinity);

            var addedNewWordIds = new List<int>();
            var addedKnownWordIds = new List<int>();

            for (int i = 0; i < decades; i++)
            {                
                var addNewWordsCount = GetCountNewWordsToAdd(newWords.Count, addedNewWordIds.Count, newWordsCountOnDecade);                
                var addKnownWordsCount = GetCountKnownWordsToAdd(knownWords.Length, addedKnownWordIds.Count, addNewWordsCount);

                var addNewWords = newWords.Where(x => !addedNewWordIds.Contains(x.Id)).Take(addNewWordsCount).ToList();

                foreach (var word in addNewWords)
                {
                    WordActionData.Create(word)
                        .AddTo(result);

                    addedNewWordIds.Add(word.Id);
                }

                AddKnownWords(knownWords, addedKnownWordIds, addKnownWordsCount, result);
            }
        }

        private static int GetCountNewWordsToAdd(int newWordsCount, int addedNewWordsCount, int newWordsCountOnDecade)
        {
            var remainNewWordsCount = newWordsCount - addedNewWordsCount;

            return remainNewWordsCount > newWordsCountOnDecade ? newWordsCountOnDecade : remainNewWordsCount;
        }

        private static int GetCountKnownWordsToAdd(int knownWordsCount, int addedKnownWordsCount, int addNewWordsCount)
        {
            var knownWordsOnDecade = 10 - addNewWordsCount;
            var remainKnownWordsCount = knownWordsCount - addedKnownWordsCount;

            return remainKnownWordsCount > knownWordsOnDecade ? knownWordsOnDecade : remainKnownWordsCount;
        }

        private void AddKnownWords(LanguageWord[] knownWords, List<int> addedIds, int needCount, List<WordActionData> result)
        {
            var addedCount = 0;

            for (int i = 0; i < knownWords.Length; i++)
            {
                if(addedCount == needCount)
                {
                    break;
                }

                if (addedIds.Contains(knownWords[i].Id))
                {
                    continue;
                }

                int wrongWordIndex;

                while (true)
                {
                    wrongWordIndex = GetRandom(0, knownWords.Length - 1);

                    if (wrongWordIndex != i || knownWords.Length == 1)
                    {
                        break;
                    }
                }

                var side = (WordActionData.Side)GetRandom((int)WordActionData.Side.Left, (int)WordActionData.Side.Right);

                WordActionData.Create(knownWords[i], knownWords[wrongWordIndex], side)
                    .AddTo(result);

                addedIds.Add(knownWords[i].Id);

                addedCount++;
            }
        }

        private string GetSqlCountNewWords()
        {
            if(getCountNewWordsSqlDictionary.ContainsKey(currentLanguage))
            {
                return getCountNewWordsSqlDictionary[currentLanguage];
            }

            var sql = GetSqlCount(true);

            getCountNewWordsSqlDictionary.Add(currentLanguage, sql);

            return sql;
        }

        private string GetSqlCount(bool isNewWords)
        {
            var condition = isNewWords ? "[t].count IS NULL" : "[t].count IS NOT NULL";

            return $"SELECT count(*) as NewWordsCount FROM [language_words] as [lw] " +
                $" LEFT JOIN (" +
                $"      SELECT [wa].LanguageWordId, count(*) as count FROM [word_activities] as [wa] " +
                $"      JOIN [language_words] as [lwInn] ON [wa].LanguageWordId = [lwInn].Id " +
                $"      WHERE [lwInn].LanguageId = {currentLanguage.LanguageId} " +
                $"      AND [lwInn].TranslationLanguageId = {currentLanguage.TranslationLanguageId}" +
                $"      GROUP BY [wa].LanguageWordId " +
                $") as [t] ON [lw].Id = [t].LanguageWordId " +
                $" WHERE [lw].LanguageId = {currentLanguage.LanguageId} " +
                $" AND [lw].TranslationLanguageId = {currentLanguage.TranslationLanguageId} " +
                $" AND {condition} ";
        }

        private string GetSqlCountKnownWords()
        {
            if (getCountKnownWordsSqlDictionary.ContainsKey(currentLanguage))
            {
                return getCountKnownWordsSqlDictionary[currentLanguage];
            }

            var sql = GetSqlCount(false);

            getCountKnownWordsSqlDictionary.Add(currentLanguage, sql);

            return sql;
        }

        private int GetCountAllNewWords(WordActionContext db)
        {
            return db.Database.SqlQueryRaw<int>(GetSqlCountNewWords()).ToList().FirstOrDefault();
        }

        private int GetCountAllKnownWords(WordActionContext db)
        {
            return db.Database.SqlQueryRaw<int>(GetSqlCountKnownWords()).ToList().FirstOrDefault();
        }

        private int CalculateNewWordsCount(int count, int allNewWords, int allKnownWords)
        {
            if(allNewWords == 0)
            {
                return 0;
            }

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

            if(allNewWords < count)
            {
                if(allNewWords < count * 0.1)
                {
                    return allNewWords;
                }
            }

            // 60% вероятность отклонения запроса новых слов
            if(GetRandom(0, 10) < 7)
            {
                return 0;
            }

            if(count < 21)
            {
                var result = (int)Math.Round(count * 0.25);

                return result <= allNewWords ? result : allNewWords;
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

            var date = TimeHelper.GetCurrentDate().BeginDay();

            var sql = $" SELECT [lw].* FROM [language_words] as [lw] " +
                $" LEFT JOIN ( " +
                $"          SELECT [wa].LanguageWordId, count(*) as count FROM [word_activities] as [wa] " +
                $"          JOIN [language_words] as [lwInn] ON [wa].LanguageWordId = [lwInn].Id " +
                $"          WHERE [lwInn].LanguageId = {currentLanguage.LanguageId} " +
                $"          AND [lwInn].TranslationLanguageId = {currentLanguage.TranslationLanguageId} " +
                $"          GROUP BY [wa].LanguageWordId " +
                $"          ) " +
                $" as t ON lw.Id = t.LanguageWordId " +
                $" LEFT JOIN ( " +
                $"           SELECT lwInn.Id, IIF(we.Id IS NULL, 0, 1) as hasError " +
                $"           FROM [language_words] as [lwInn] " +
                $"           LEFT JOIN [word_activity_errors] [we] ON we.LanguageWordId == lwInn.Id " +
                $"           LEFT JOIN [word_actions] as [wa] ON we.InfoId = wa.Id " +
                $"           WHERE [lwInn].LanguageId = {currentLanguage.LanguageId} " +
                $"           AND [lwInn].TranslationLanguageId = {currentLanguage.TranslationLanguageId} " +
                $"           AND wa.EndDate < {date.ToSqlString()} " +
                $" ) as lwe ON lwe.Id = lw.Id " +
                $" WHERE [lw].LanguageId = {currentLanguage.LanguageId} " +
                $" AND [lw].TranslationLanguageId = {currentLanguage.TranslationLanguageId} " +
                $" AND t.count IS NOT NULL " +
                $" ORDER BY lwe.hasError DESC, t.count ASC " +
                $" LIMIT {count} OFFSET {offset} ";

            return db.LanguageWords.FromSqlRaw(sql).ToArray();
        }

        private List<LanguageWord> LoadNewWords(WordActionContext db, int count, int allNewWords)
        {
            var idArr = LoadNewWordIds(db, count, allNewWords);

            var ids = SelectNewWordIds(idArr, count);

            return db.LanguageWords.Where(x => ids.Contains(x.Id)).ToList();
        }

        private int[] LoadNewWordIds(WordActionContext db, int count, int allNewWords)
        {
            if(count == 0)
            {
                return Array.Empty<int>();
            }

            var offset = allNewWords > count ? GetRandom(0, allNewWords - count) : 0;

            var sql = $"SELECT Id FROM [language_words] as [lw] " +
                $" LEFT JOIN (" +
                $"     SELECT [wa].LanguageWordId, count(*) as count FROM [word_activities] as [wa] " +
                $"      JOIN [language_words] as [lwInn] ON [wa].LanguageWordId = [lwInn].Id " +
                $"      WHERE [lwInn].LanguageId = {currentLanguage.LanguageId} " +
                $"      AND [lwInn].TranslationLanguageId = {currentLanguage.TranslationLanguageId}" +
                $"      GROUP BY [wa].LanguageWordId " +
                $") as t ON lw.Id = t.LanguageWordId " +
                $" WHERE [lw].LanguageId = {currentLanguage.LanguageId} " +
                $" AND [lw].TranslationLanguageId = {currentLanguage.TranslationLanguageId} " +
                $" AND t.count IS NULL " +
                $" LIMIT {count} OFFSET {offset}";

            return db.Database.SqlQueryRaw<int>(sql).ToArray();
        }

        private List<int> SelectNewWordIds(int[] ids, int count)
        {
            var result = new List<int>();

            if(ids.Length == 0)
            {
                return result;
            }

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
