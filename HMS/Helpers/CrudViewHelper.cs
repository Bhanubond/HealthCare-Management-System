using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HMS.Helpers
{
    public static class CrudViewHelper
    {
        private static readonly string[] FriendlyNestedPropertyNames =
        {
            "Name",
            "DisplayName",
            "Title",
            "CourseName",
            "DeptName",
            "DesignationName",
            "StateName",
            "MandalName",
            "CityName",
            "ScreenDisplayName",
            "UserName",
            "YearName"
        };

        private static readonly string[] SensitiveDisplayPropertyTokens =
        {
            "Password",
            "Secret",
            "Token",
            "Hash",
            "Otp",
            "Pin"
        };

        public static PropertyInfo? GetKeyProperty(Type modelType)
        {
            return modelType.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null)
                ?? modelType.GetProperties().FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<PropertyInfo> GetDisplayProperties(Type modelType)
        {
            return modelType.GetProperties()
                .Where(p => p.GetCustomAttribute<ScaffoldColumnAttribute>()?.Scaffold != false)
                .Where(p => p.GetIndexParameters().Length == 0)
                .Where(p => p.GetMethod != null)
                .Where(p => !ShouldSkipDisplayProperty(modelType, p));
        }

        public static IEnumerable<PropertyInfo> GetEditableProperties(Type modelType, ISet<string> hiddenProperties)
        {
            return modelType.GetProperties()
                .Where(p => p.GetIndexParameters().Length == 0)
                .Where(p => p.SetMethod != null)
                .Where(p => !IsNavigationProperty(p))
                .Where(p => !hiddenProperties.Contains(p.Name))
                .Where(p => !string.Equals(GetKeyProperty(modelType)?.Name, p.Name, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsSelectList(ViewDataDictionary viewData, string propertyName)
        {
            if (!viewData.ContainsKey(propertyName))
            {
                return false;
            }

            return viewData[propertyName] is IEnumerable<SelectListItem> || viewData[propertyName] is SelectList;
        }

        public static IEnumerable<SelectListItem>? GetSelectList(ViewDataDictionary viewData, string propertyName)
        {
            if (!viewData.ContainsKey(propertyName))
            {
                return null;
            }

            return viewData[propertyName] as IEnumerable<SelectListItem>
                ?? (viewData[propertyName] as SelectList)?.Cast<SelectListItem>();
        }

        public static string GetLabel(PropertyInfo property)
        {
            var display = property.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? property.Name;
        }

        public static string GetDisplayValue(object? value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is bool boolean)
            {
                return boolean ? "Yes" : "No";
            }

            if (value is DateTime dateTime)
            {
                return dateTime.ToString("O");
            }

            var valueType = value.GetType();
            if (IsSimpleType(valueType))
            {
                return Convert.ToString(value) ?? string.Empty;
            }

            foreach (var nestedName in FriendlyNestedPropertyNames)
            {
                var nestedProperty = valueType.GetProperty(nestedName);
                if (nestedProperty?.GetValue(value) is string nestedText && !string.IsNullOrWhiteSpace(nestedText))
                {
                    return nestedText;
                }
            }

            var stringProperty = valueType.GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(string) && p.GetIndexParameters().Length == 0);

            if (stringProperty?.GetValue(value) is string text)
            {
                return text;
            }

            return string.Empty;
        }

        public static string GetInputValue(object? value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is bool boolean)
            {
                return boolean ? "true" : "false";
            }

            if (value is DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-dd");
            }

            return Convert.ToString(value) ?? string.Empty;
        }

        public static object? GetPropertyValue(object model, PropertyInfo property)
        {
            return property.GetValue(model);
        }

        public static bool ShouldSkipDisplayProperty(Type modelType, PropertyInfo property)
        {
            if (property.GetIndexParameters().Length > 0)
            {
                return true;
            }

            if (SensitiveDisplayPropertyTokens.Any(token => property.Name.Contains(token, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            if (IsNavigationProperty(property))
            {
                return false;
            }

            if (!IsSimpleType(UnwrapNullable(property.PropertyType)))
            {
                return true;
            }

            if (!property.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var baseName = property.Name[..^2];
            return modelType.GetProperty(baseName) != null;
        }

        private static bool IsNavigationProperty(PropertyInfo property)
        {
            var propertyType = UnwrapNullable(property.PropertyType);
            return propertyType != typeof(string)
                   && !IsSimpleType(propertyType)
                   && !typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType);
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type == typeof(DateTime)
                   || type == typeof(DateOnly)
                   || type == typeof(TimeOnly)
                   || type == typeof(Guid);
        }

        private static Type UnwrapNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }
}
