﻿using WordCards.Core.Commands;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using WordCards.Core.Validations;
using WordCards.Business.Languages;

namespace WordCards.Configurations
{
    public sealed class SaveSettingsCommand : EntityCommand, ISaveSettingsCommand
    {
        public bool Execute(Settings settings)
        {
            Validate(settings);

            PrepareToSave(settings);

            using (var db = new ConfigurationContext())
            {
                var configurations = db.Configurations.ToDictionary(x => x.Id);

                UpdateProperty(db, configurations, x => x.CurrentLanguage, settings.CurrentLanguage.Id);
                UpdateProperty(db, configurations, x => x.CurrentTranslationLanguage, settings.TranslationLanguage.Id);
                UpdateProperty(db, configurations, x => x.WordCardHasTimer, settings.WordCardHasTimer);
                UpdateProperty(db, configurations, x => x.WordCardTimerDurationInSeconds, settings.WordCardTimerDurationInSeconds);

                db.SaveChanges();
            }

            AppConfiguration.Refresh();

            return true;
        }

        private static void Validate(Settings settings)
        {
            var validationResult = new ValidationResult();

            ValidateInternal(settings, validationResult);

            validationResult.ThrowIfHasError();
        }

        private static void ValidateInternal(Settings settings, ValidationResult validationResult)
        {
            if (settings.CurrentLanguage == null)
            {
                validationResult.AddRequired<Settings, Language>(x => x.CurrentLanguage);

                return;
            }

            if (settings.TranslationLanguage == null)
            {
                validationResult.AddRequired<Settings, Language>(x => x.TranslationLanguage);

                return;
            }

            if(settings.CurrentLanguage.Id == settings.TranslationLanguage.Id)
            {
                validationResult.Add("Язык изучения и язык перевода не могут быть одинаковыми");
            }

            if(settings.WordCardHasTimer && settings.WordCardTimerDurationInSeconds <= 0)
            {
                validationResult.AddGreaterThan<Settings, int>(x => x.WordCardTimerDurationInSeconds, 0);                
            }            
        }

        private static void PrepareToSave(Settings settings)
        {
            if(!settings.WordCardHasTimer)
            {
                settings.WordCardTimerDurationInSeconds = 0;
            }
        }

        private static void UpdateProperty<TProperty>(ConfigurationContext db, Dictionary<string, Configuration> configurations, 
            Expression<Func<AppConfiguration, TProperty>> expr, object newValue)
        {
            var configId = GetConfigurationId(expr);

            var configuration = configurations[configId];

            var valueAsString = newValue.ToString();

            if(configuration.Value != valueAsString)
            {
                configuration.Value = valueAsString;

                db.Update(configuration);
            }
        }

        private static string GetConfigurationId<TProperty>(Expression<Func<AppConfiguration, TProperty>> expr)
        {
            var type = typeof(AppConfiguration);
            var name = (expr.Body as MemberExpression).Member.Name;

            var property = type.GetProperty(name);

            var attributes = property.GetCustomAttributes(false);

            foreach (var attr in attributes)
            {
                if (attr is ConfigurationIdAttribute idAttr)
                {
                    return idAttr.Id;
                }
            }

            return string.Empty;
        }
    }
}
