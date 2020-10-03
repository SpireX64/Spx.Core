using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Spx.Reflection.UnitTests
{
    public class ClassTest
    {
        [Fact]
        [SuppressMessage("ReSharper", "CheckForReferenceEqualityInstead.1")]
        public void Class_GetClassOfStringBuilder_ReturnStringBuilderType()
        {
            var cls = Class<ClassTest>.Get();
            var type = typeof(ClassTest);
            
            Assert.True(type.Equals(cls));
            Assert.True(cls.Equals(type));
        }

        [Fact]
        public void Class_TryCastValueTypeToClass_ThrowException()
        {
            var intType = typeof(int);
            
            var exception = Assert.Throws<ArgumentException>(() => (Class<object>) intType);
            Assert.Contains("is not a class", exception.Message);
        }

        [Fact]
        public void Class_TryCastOneClassToAnother_ThrowException()
        {
            var classAssert = typeof(Assert);

            var exception = Assert.Throws<ArgumentException>(() => (Class<ClassTest>) classAssert);
            Assert.Contains("is not assignable from", exception.Message);
        }

        [Fact]
        public void Class_CheckAreTypeHashCodesEqual_HashCodesAreEqual()
        {
            var typeAssert = typeof(Assert);
            var classAssert = Class<Assert>.Get();
            
            Assert.Equal(
                typeAssert.GetHashCode(),
                classAssert.GetHashCode()
            );
        }

        [Fact]
        public void Class_CheckHashCodesOfSameClasses_AreEqual()
        {
            var hash1 = Class<ClassTest>.Get().GetHashCode();
            var hash2 = Class<ClassTest>.Get().GetHashCode();
            
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void Class_CheckTypes()
        {
            object cls1 = Class<ClassTest>.Get();
            object cls2 = Class<Assert>.Get();
            
            Assert.False(cls1.Equals(cls2));
            Assert.False(cls2.Equals(cls1));
        }

        [Fact]
        public void Class_ClassAndSameTypeEqual_AreEqual()
        {
            var typeAssert = typeof(Assert);
            var classAssert = Class<Assert>.Get();
            
            Assert.True(classAssert == typeAssert);
            Assert.True(typeAssert.Equals(classAssert));
        }

        [Fact]
        public void Class_CheckEquals_ClassesAreEqual()
        {
            var classAssert1 = Class<Assert>.Get(); 
            var classAssert2 = Class<Assert>.Get();
            
            Assert.True(classAssert1.Equals(classAssert1));
            Assert.True(classAssert1.Equals(classAssert2));
            Assert.True(classAssert2.Equals(classAssert1));
            Assert.True(classAssert2.Equals(classAssert2));
        }

        [Fact]
        public void Class_ClassNotEqualWithOtherObjects_NotEqual()
        {
            var classAssert = Class<Assert>.Get();
            
            Assert.False(classAssert.Equals(new object()));
            Assert.False(classAssert.Equals(null));
        }
    }
}