﻿using System.Collections.ObjectModel;
using System.Linq;
using WordCards.Core.Commands;

namespace WordCards.Business.Languages
{
    public sealed class GetLanguagesCommand : EntityCommand, IGetLanguagesCommand
    {
        public ObservableCollection<Language> Execute()
        {
            ObservableCollection<Language> result;

            using (var db = new LanguageContext())
            {
                result = new ObservableCollection<Language>(db.Languages.OrderBy(x => x.Name).ToList());
            }

            return result;
        }
    }
}
