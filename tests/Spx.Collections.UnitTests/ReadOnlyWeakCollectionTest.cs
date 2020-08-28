using Spx.Collections.UnitTests.Utils;
using Spx.Collections.Weak;
using Xunit;

namespace Spx.Collections.UnitTests
{
    public class ReadOnlyWeakCollectionTest
    {
        [Fact]
        public void ReadOnlyWeakCollection_FromWeakCollection_ReturnsReadOnlyCollection()
        {
            // ---- Arrange
            var item = new DummyEntity(1);
            var weakCollection = new WeakCollection<DummyEntity> {item};
            
            // ---- Act
            var readOnlyCollection = weakCollection.ToReadOnly();
            
            // ---- Assert
            Assert.IsAssignableFrom<ReadOnlyWeakCollection<DummyEntity>>(readOnlyCollection);
        }

        [Fact]
        public void ReadOnlyWeakCollection_FromWeakSet_ReturnsReadOnlyCollection()
        {
            // ---- Arrange
            var item = new DummyEntity(0);
            var weakSet = new WeakSet<DummyEntity> {item};
            
            // ---- Act
            var readOnlyCollection = weakSet.ToReadOnly();
            
            // ---- Assert
            Assert.NotEmpty(readOnlyCollection);
            Assert.IsAssignableFrom<ReadOnlyWeakCollection<DummyEntity>>(readOnlyCollection);
        }

        [Fact]
        public void ReadOnlyWeakCollection_TryCastToMutableCollection_FailToCast()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object>();
            var readOnlyCollection = weakCollection.ToReadOnly();
            
            // ---- Act
            var casted = readOnlyCollection as WeakCollection<DummyEntity>;
            
            // ---- Assert
            Assert.Null(casted);
        }

        [Fact]
        public void ReadOnlyWeakCollection_TryCastToMutableSet_FailToCast()
        {
            // ---- Arrange
            var weakSet = new WeakSet<object>();
            var readOnlyCollection = weakSet.ToReadOnly();
            
            // ---- Act
            var casted = readOnlyCollection as WeakSet<object>;
            
            // ---- Assert
            Assert.Null(casted);
        }

        [Fact]
        public void ReadOnlyWeakCollection_ContainsItem_ReturnsTrue()
        {
            // ---- Arrange
            var item = new object();
            var readOnlyWeakCollection = new WeakCollection<object>{ item }.ToReadOnly() as ReadOnlyWeakCollection<object>;
            
            // ---- Assert
            Assert.True(readOnlyWeakCollection!.Contains(item));
        }

        [Fact]
        public void ReadOnlyWeakCollection_GetCount_CountNumbersAreSame()
        {
            // ---- Arrange
            var item = new object();
            var collection = new WeakCollection<object> {item};
            var readOnly = (ReadOnlyWeakCollection<object>) collection.ToReadOnly();
            
            // ---- Assert
            Assert.Equal(collection.Count, readOnly.Count);
        }

        [Fact]
        public void ReadOnlyWeakCollection_Enumerator()
        {
            // ---- Arrange
            var item = new object();
            var readOnlyWeakCollection = new WeakCollection<object>{ item }.ToReadOnly();
            var enumerator = readOnlyWeakCollection.GetEnumerator();
            
            // ---- Act
            enumerator.MoveNext();
            var itemFromColletion = enumerator.Current;

            // ---- Assert
            Assert.Equal(item, itemFromColletion);
        }
    }
}