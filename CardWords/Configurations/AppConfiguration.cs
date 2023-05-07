﻿using CardWords.Core;
using CardWords.Core.Helpers;
using System;
using System.Collections.Generic;

namespace CardWords.Configurations
{
    public sealed class AppConfiguration
    {
        private static AppConfiguration? instance;

        private AppConfiguration(IReadOnlyDictionary<Id, Configuration> data) 
        {
            SetProperties(data);
        }

        public static AppConfiguration GetInstance()
        {
            if(instance == null)
            {
                instance = Load();
            }

            return instance;
        }

        public static void Refresh()
        {
            if (instance == null)
            {
                instance = Load();

                return;
            }

            var data = CommandHelper.GetCommand<ILoadConfigurationCommand>().Execute();

            instance.SetProperties(data);
        }

        private static AppConfiguration Load()
        {
            var data = CommandHelper.GetCommand<ILoadConfigurationCommand>().Execute();

            return new AppConfiguration(data);
        }

        [IdConfiguration("current_language")]
        public Id CurrentLanguage { get; private set; }

        private void SetProperties(IReadOnlyDictionary<Id, Configuration> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }            

            var properties = typeof(AppConfiguration).GetProperties(System.Reflection.BindingFlags.Public);

            foreach ( var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);

                foreach (var attr in attributes)
                {
                    if(attr is IdConfigurationAttribute idAttr)
                    {
                        var configuration = data[idAttr.Id];

                        if(configuration == null)
                        {
                            throw new Exception("Not found configuration");
                        }

                        var value = Convert.ChangeType(configuration.Value, property.PropertyType);

                        property.SetValue(this, value);
                    }
                }
            }
        }
    }
}