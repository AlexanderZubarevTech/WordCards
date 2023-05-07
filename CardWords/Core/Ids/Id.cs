using CardWords.Core.Exceptions;
using CardWords.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace CardWords.Core.Ids
{
    public sealed class IdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (Id.Types.ContainsKey(sourceType))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (Id.TryParse(value, out Id newId))
            {
                return newId;
            }

            return base.ConvertFrom(context, culture, value);
        }        

        public override bool IsValid(ITypeDescriptorContext? context, object? value)
        {
            if(value != null && Id.Types.ContainsKey(value.GetType()))
            {
                return true;
            }

            return base.IsValid(context, value);
        }        
    }

    [TypeConverter(typeof(IdTypeConverter))]
    public readonly partial struct Id : IComparer<Id>, IComparable<Id>, IEquatable<Id>
    {
        public enum EnumTypes
        {
            Int, Long, String
        }

        private static readonly IReadOnlyDictionary<EnumTypes, Type> typesByEnum = new Dictionary<EnumTypes, Type>
        {
            { EnumTypes.Int, typeof(int) },
            { EnumTypes.Long, typeof(long) },
            { EnumTypes.String, typeof(string) },
        };

        public static readonly IReadOnlyDictionary<Type, EnumTypes> Types = typesByEnum.ToDictionary(x => x.Value, x => x.Key);

        public static readonly Id Empty = default;
        public static readonly Id Zero = new Id(0);


        public static Id Create(int id)
        {
            if (id < 0)
            {
                throw new Exception();
            }

            return id == 0 ? Empty : new Id(id);
        }

        public static Id Create(long id)
        {
            if (id <= 0)
            {
                throw new Exception();
            }

            return new Id(id);
        }

        public static Id Create(string id)
        {
            if (id == null || id.IsNullOrEmptyOrWhiteSpace())
            {
                return Empty;
            }

            var isNumber = long.TryParse(id, null, out long idNumber);

            if (isNumber)
            {
                if (idNumber < 0)
                {
                    throw new Exception();
                }

                if (idNumber == 0)
                {
                    return Empty;
                }
            }

            return new Id(id, isNumber);
        }

        public Id()
            : this(default, default, default)
        {
        }

        private Id(int id)
            : this(id, EnumTypes.Int, true)
        {
        }

        private Id(long id)
            : this(id, EnumTypes.Long, true)
        {
        }

        private Id(string id, bool isNumber)
            : this(id, EnumTypes.String, isNumber)
        {
        }

        private Id(object id, EnumTypes enumType, bool isNumber)
        {
            Value = id;
            this.enumType = enumType;
            Type = typesByEnum[enumType];
            IsNumber = isNumber;
        }


        private readonly EnumTypes enumType;

        public object Value { get; }

        public Type Type { get; }

        public bool IsNumber { get; }

        public bool IsNotNumber => !IsNumber;

        public bool IsEmpty => Equals(Empty);

        public bool IsNotEmpty => !IsEmpty;

        public int Compare(Id x, Id y)
        {
            if (x > y)
            {
                return 1;
            }

            return x == y ? 0 : -1;
        }

        public int CompareTo(Id other)
        {
            return Compare(this, other);
        }

        public bool Equals(Id other)
        {
            if (enumType != other.enumType)
            {
                return false;
            }

            if (Type != other.Type)
            {
                return false;
            }

            if (Value == null && other.Value == null)
            {
                return true;
            }

            if (Value == null || other.Value == null)
            {
                return false;
            }

            return Value.Equals(other.Value);
        }

        #region Operators

        public static bool operator >(Id id1, Id id2)
        {
            if (id1 == Empty)
            {
                return false;
            }

            if (id2 == Empty)
            {
                return true;
            }

            if (id1.IsNotNumber || id2.IsNotNumber)
            {
                return CompareStringIds(id1, id2) > 0;
            }

            return id1.As<long>() > id2.As<long>();
        }

        private static int CompareStringIds(Id id1, Id id2)
        {
            return string.Compare(id1.Value.ToString(), id2.Value.ToString(), true);
        }

        public static bool operator <(Id id1, Id id2)
        {
            if (id2 == Empty)
            {
                return false;
            }

            if (id1 == Empty)
            {
                return true;
            }

            return !(id1 > id2);
        }

        public static bool operator ==(Id id1, Id id2)
        {
            if (id1.IsEmpty && id2.IsEmpty)
            {
                return true;
            }

            if (id1.IsEmpty || id2.IsEmpty)
            {
                return false;
            }

            if (id1.IsNotNumber || id2.IsNotNumber)
            {
                return CompareStringIds(id1, id2) == 0;
            }

            return id1.As<long>() == id2.As<long>();
        }

        public static bool operator !=(Id id1, Id id2)
        {
            return !(id1 == id2);
        }

        public static bool operator >=(Id id1, Id id2)
        {
            if (id1 == id2)
            {
                return true;
            }

            return id1 > id2;
        }

        public static bool operator <=(Id id1, Id id2)
        {
            if (id1 == id2)
            {
                return true;
            }

            return id1 < id2;
        }

        public static implicit operator Id(int id)
        {
            return Create(id);
        }

        public static implicit operator Id(long id)
        {
            return Create(id);
        }

        public static implicit operator Id(string id)
        {
            return Create(id);
        }

        public static explicit operator int(Id id)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException();
            }

            return id.enumType == EnumTypes.Int ? (int)id.Value : throw new Exception();
        }

        public static explicit operator long(Id id)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException();
            }

            return id.enumType == EnumTypes.Long ? (long)id.Value : throw new Exception();
        }

        public static explicit operator string(Id id)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException();
            }

            return id.enumType == EnumTypes.String ? (string)id.Value : throw new Exception();
        }

        #endregion

        public static bool TryParse<T>(T value, out Id newId)
        {
            newId = Empty;

            try
            {
                newId = Parse(value);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool TryParse(object? value, out Id newId)
        {
            newId = Empty;

            try
            {
                newId = Parse(value);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static Id Parse<T>(T value)
        {
            var valueType = typeof(T);

            if (value == null || !Types.ContainsKey(valueType))
            {
                throw new UnknownTypeException(valueType);
            }

            var type = Types[valueType];

            switch (type)
            {
                case EnumTypes.Int:
                    {
                        return Create(Convert.ToInt32(value));
                    }
                case EnumTypes.Long:
                    {
                        return Create(Convert.ToInt64(value));
                    }
                case EnumTypes.String:
                    {
                        return Create(value.ToString());
                    }
                default:
                    {
                        return Empty;
                    }
            }
        }

        public static Id Parse(object? value)
        {
            if (value == null)
            {
                return Empty;
            }

            if (IsId(value))
            {
                return (Id)value;
            }

            var valueType = value.GetType();

            if (!Types.ContainsKey(valueType))
            {
                throw new UnknownTypeException(valueType);
            }

            var valueEnumType = Types[valueType];

            switch (valueEnumType)
            {
                case EnumTypes.Int:
                    {
                        return Create((int)value);
                    };
                case EnumTypes.Long:
                    {
                        return Create((long)value);
                    };
                case EnumTypes.String:
                    {
                        return Create((string)value);
                    };
                default:
                    {
                        return Empty;
                    };
            }
        }

        public static bool IsId(object value)
        {
            if (value == null)
            {
                return false;
            }

            return value is Id;
        }

        public static bool ExistType<T>()
        {
            return Types.ContainsKey(typeof(T));
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (!IsId(obj))
            {
                return false;
            }

            var id = obj as Id?;

            return Equals(id.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ Type.GetHashCode() ^ IsNumber.GetHashCode() ^ enumType.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value?.ToString() ?? string.Empty}";
        }
    }
}
