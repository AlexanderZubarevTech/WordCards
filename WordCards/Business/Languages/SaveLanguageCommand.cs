using System.Linq;
using WordCards.Core.Commands;
using WordCards.Core.Validations;
using WordCards.Extensions;

namespace WordCards.Business.Languages
{
    public sealed class SaveLanguageCommand : EntityCommand, ISaveLanguageCommand
    {
        public bool Execute(Language entity)
        {
            Validate(entity);

            Prepare(entity);

            using (var db = new LanguageContext())
            {
                var name = entity.Name.ToLower();

                var existEntity = db.Languages.ToList().FirstOrDefault(x => x.Name.ToLower() == name);

                if (existEntity != null)
                {
                    ValidationResult.ThrowError<Language, string>("Данное название языка уже существует", x => x.Name);
                }

                if (entity.Id == default)
                {
                    db.Languages.Add(entity);
                }
                else
                {
                    db.Languages.Update(entity);
                }

                db.SaveChanges();
            }

            return true;
        }

        private static void Prepare(Language entity)
        {
            entity.Name = entity.Name.Trim();
        }

        private static void Validate(Language entity)
        {
            var validationResult = new ValidationResult();

            if (entity.Name.IsNullOrEmptyOrWhiteSpace())
            {
                validationResult.AddRequired<Language, string>(x => x.Name);
            }

            validationResult.ThrowIfHasError();
        }
    }
}
