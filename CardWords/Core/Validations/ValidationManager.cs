using CardWords.Core.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using CardWords.Core.Exceptions;
using CardWords.Core.Helpers;

namespace CardWords.Core.Validations
{
    public sealed partial class ValidationManager
    {
        private const string fieldsElementTag = "Fields";
        private const string validationElementTag = "Validation";
        private const string fieldPropertyNameTag = "FieldPropertyName";
        private const string fieldNameTag = "FieldName";
        private const string fieldValueTag = "FieldValue";
        private const string fieldBackgroundTag = "FieldBackground";

        public ValidationManager(FrameworkElement mainElement, Style errorStyle, Color errorBacgroundColor)
        {
            this.errorStyle = errorStyle;            

            Initialize(mainElement, errorBacgroundColor);
        }

        private readonly Style errorStyle;

        private StackPanel errorMessagePanel;

        private IReadOnlyDictionary<string, ValidationField> fields;        

        #region Initialize

        private void Initialize(FrameworkElement mainElement, Color errorBacgroundColor)
        {
            var elements = LogicalTreeHelper.GetChildren(mainElement);

            FrameworkElement? fieldsElement = null;
            FrameworkElement? errorMessageElement = null;

            foreach (var item in elements)
            {
                if (item != null && item is FrameworkElement element)
                {
                    var tags = ElementTag.ParseTag(element.Tag);

                    if (tags.ContainsKey(fieldsElementTag))
                    {
                        fieldsElement = element;
                    }

                    if (tags.ContainsKey(validationElementTag))
                    {
                        errorMessageElement = element;
                    }
                }
            }

            if (errorMessageElement == null)
            {
                throw new Exception("Элемент для вывода ошибок не найден");
            }

            if (errorMessageElement is StackPanel errorPanel)
            {
                errorMessagePanel = errorPanel;
            }

            fields = fieldsElement != null 
                ? GetFields(fieldsElement, errorBacgroundColor) 
                : DictionaryHelper.Empty<string, ValidationField>();
        }

        private Dictionary<string, ValidationField> GetFields(FrameworkElement fieldsElement, Color errorBacgroundColor)
        {
            var result = new List<ValidationField>();

            var elements = LogicalTreeHelper.GetChildren(fieldsElement);

            foreach (var item in elements)
            {
                if(item is FrameworkElement element)
                {
                    var field = GetField(element, errorBacgroundColor);

                    if(field != null)
                    {
                        result.Add(field);
                    }
                }
            }

            return result.ToDictionary(x => x.FieldPropertyName);
        }

        private ValidationField? GetField(FrameworkElement element, Color errorBacgroundColor)
        {
            var fieldElement = GetElement(element, fieldPropertyNameTag);

            if(fieldElement == null)
            {
                return null;
            }

            var propName = ElementTag.ParseTag(fieldElement.Tag)[fieldPropertyNameTag].Value;

            var fieldNameElement = GetElement(fieldElement, fieldNameTag);

            if(fieldNameElement == null)
            {
                throw new Exception($"Тег {fieldNameTag} у свойства {propName} не найден");
            }

            var fieldName = (fieldNameElement as TextBlock).Text;

            var fieldValueElement = GetElement(fieldElement, fieldValueTag);

            if (fieldValueElement == null)
            {
                throw new Exception($"Тег {fieldValueTag} у свойства {propName} не найден");
            }

            var fieldBackgroundElement = GetElement(fieldElement, fieldBackgroundTag);

            if(fieldBackgroundElement == null)
            {
                return new ValidationField(propName, fieldName, fieldValueElement);
            }

            return new ValidationField(propName, fieldName, fieldValueElement, fieldBackgroundElement, errorBacgroundColor);
        }

        private FrameworkElement? GetElement(FrameworkElement element, string tag)
        {
            var isFound = false;

            return GetElementRecursive(element, tag, ref isFound);
        }

        private FrameworkElement? GetElementRecursive(FrameworkElement parentElement, string tag, ref bool isFound)
        {
            var tags = ElementTag.ParseTag(parentElement.Tag);

            if (tags.ContainsKey(tag))
            {
                isFound = true;

                return parentElement;
            }

            var elements = LogicalTreeHelper.GetChildren(parentElement);

            foreach (var item in elements)
            {
                if (item is FrameworkElement element)
                {                    
                    if(isFound)
                    {
                        return null;
                    } 
                    else
                    {
                        var result = GetElementRecursive(element, tag, ref isFound);

                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        public bool Execute(Action executeAction)
        {
            SetDefault();

            bool isValid = true;

            try
            {
                executeAction.Invoke();
            }
            catch (ValidationResultException ex)
            {
                if(ex.ValidationResult != null)
                {
                    Validate(ex.ValidationResult);
                }
                else
                {
                    AddErrorMessage(ex.Message);
                }

                isValid = false;
            }

            return isValid;
        }

        private void SetDefault()
        {
            foreach (var field in fields.Values)
            {
                field.SetDefault();
            }

            errorMessagePanel.Children.Clear();
        }

        private void Validate(ValidationResult validationResult)
        {
            foreach(var error in validationResult)
            {
                if(error.HasField)
                {
                    var field = fields[error.PropertyName];

                    field.SetError();

                    var message = error.GetMessage(field.FieldName);

                    AddErrorMessage(message);
                }
                else
                {
                    AddErrorMessage(error.Message);
                }
            }
        }

        private void AddErrorMessage(string errorMessage)
        {
            var block = new TextBlock();
            block.Style = errorStyle;
            block.Text = errorMessage;

            errorMessagePanel.Children.Add(block);
        }
    }
}
