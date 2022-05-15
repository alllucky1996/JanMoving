using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;

namespace Smartstore.Moving.Extensions
{
    public static class MigrationExtentions
    {
        public static ICreateTableColumnOptionOrWithColumnSyntax UsingColumn(this ICreateTableWithColumnSyntax syntax, PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            ICreateTableColumnOptionOrWithColumnSyntax result;
            var _syntax = syntax.WithColumn(name);
            var propType = propertyInfo.PropertyType;
            if (propType == typeof(string))
            {
                int stringLength = 0;
                var stringLengthAttr = propertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), true)?.FirstOrDefault();
                if (stringLengthAttr != null)
                {
                    stringLength = (stringLengthAttr as StringLengthAttribute).MaximumLength;
                }
                if (stringLength > 0)
                {
                    result = _syntax.AsString(stringLength);
                }
                else
                {
                    result = _syntax.AsString();
                }

                if (Nullable.GetUnderlyingType(propType) != null)
                {
                    result = result.Nullable();
                }
                else
                {
                    result = result.NotNullable();
                }
            }
            else if (propType == typeof(int))
            {
                if (Nullable.GetUnderlyingType(propType) != null)
                {
                    result = _syntax.AsInt32().Nullable();
                }
                else
                {
                    result = _syntax.AsInt32().NotNullable();
                }
            }
            else if (propType == typeof(bool))
            {
                if (Nullable.GetUnderlyingType(propType) != null)
                {
                    result = _syntax.AsBoolean().Nullable();
                }
                else
                {
                    result = _syntax.AsBoolean().NotNullable();
                }
            }
            else if (propType == typeof(DateTime))
            {
                if (Nullable.GetUnderlyingType(propType) != null)
                {
                    result = _syntax.AsDateTime2().Nullable();
                }
                else
                {
                    result = _syntax.AsDateTime2().NotNullable();
                }
            }
            else if (propType == typeof(long))
            {
                if (Nullable.GetUnderlyingType(propType) != null)
                {
                    result = _syntax.AsInt64().Nullable();
                }
                else
                {
                    result = _syntax.AsInt64().NotNullable();
                }
            }
            else if (propType == typeof(double))
            {
                if (Nullable.GetUnderlyingType(propType) != null)
                {
                    result = _syntax.AsDouble().Nullable();
                }
                else
                {
                    result = _syntax.AsDouble().NotNullable();
                }
            }
            else if (propType == typeof(decimal))
            {
                if (Nullable.GetUnderlyingType(propType) != null)
                {
                    result = _syntax.AsDecimal().Nullable();
                }
                else
                {
                    result = _syntax.AsDecimal().NotNullable();
                }
            }
            else
                throw new ArgumentException($"Do not soupport data type. Hãy viết thêm mở rộng cho {propType.Name}.");

         
            return result;
        }


    }

}
