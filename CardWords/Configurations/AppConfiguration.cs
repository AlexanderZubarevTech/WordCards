using CardWords.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CardWords.Configurations
{
    public sealed class AppConfiguration
    {
        private static AppConfiguration? instance;

        public static AppConfiguration Instance 
        { 
            get 
            {
                if (instance == null)
                {
                    instance = Load();
                }

                return instance;
            } 
        }

        private AppConfiguration(IReadOnlyDictionary<string, Configuration> data) 
        {
            SetProperties(data);
        }

        [ConfigurationId("current_language")]
        public int CurrentLanguage { get; private set; }

        [ConfigurationId("current_translation_language")]
        public int CurrentTranslationLanguage { get; private set; }

        [ConfigurationId("word_card_has_timer")]
        public bool WordCardHasTimer { get; private set; }

        [ConfigurationId("word_card_timer_duration_in_seconds")]
        public int WordCardTimerDurationInSeconds { get; private set; }

        public static void Refresh()
        {
            if (instance == null)
            {
                instance = Load();

                return;
            }

            var data = GetData();

            instance.SetProperties(data);
        }

        private static AppConfiguration Load()
        {
            var data = GetData();

            return new AppConfiguration(data);
        }

        private static IReadOnlyDictionary<string, Configuration> GetData()
        {
            return CommandHelper.GetCommand<ILoadConfigurationCommand>().Execute();
        }

        private void SetProperties(IReadOnlyDictionary<string, Configuration> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }

            var type = typeof(AppConfiguration);

            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach ( var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);

                foreach (var attr in attributes)
                {
                    if(attr is ConfigurationIdAttribute idAttr)
                    {
                        var configuration = data[idAttr.Id];

                        if(configuration == null)
                        {
                            throw new Exception("Not found configuration");
                        }

                        var value = TypeDescriptor.GetConverter(property.PropertyType)
                            .ConvertFrom(configuration.Value);                        

                        property.SetValue(this, value);
                    }
                }
            }
        }
    }
}
