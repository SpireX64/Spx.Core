using System;
using System.Diagnostics.CodeAnalysis;

namespace Spx.Reflection
{
    public readonly struct Class<TClass> where TClass: class
    {
        public static implicit operator Type(Class<TClass> @class) => @class.Type;

        public static implicit operator Class<TClass>(Type type) => new Class<TClass>(type);

        public static Class<TClass> Get() => new Class<TClass>(typeof(TClass));

        private Class(Type type)
        {
            var requiredType = typeof(TClass);
            if (type.IsClass || type.IsInterface)
            {
                if(type == requiredType || requiredType.IsAssignableFrom(type))
                    Type = type;
                else
                    throw new ArgumentException("Given type is not assignable from " + requiredType.Name);
            }
            else
                throw new ArgumentException("Given type is not a class or interface type");
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Type type:
                    return Type == type;
                case Class<TClass> @class:
                    return Equals(@class);
                default:
                    return false;
            }
        }

        public bool Equals(Class<TClass>? other)
        {
            return Type == other?.Type;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public Type Type { get; }
    }
}