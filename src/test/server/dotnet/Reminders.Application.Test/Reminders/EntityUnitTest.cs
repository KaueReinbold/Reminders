using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.Domain.Models;

namespace Reminders.Application.Test
{
    [TestClass]
    public class EntityUnitTest
    {
        private sealed class TestEntity : Entity<Guid>
        {
            public TestEntity(Guid id, bool isDeleted = false) : base(id, isDeleted) { }
        }

        private sealed class OtherEntity : Entity<Guid>
        {
            public OtherEntity(Guid id) : base(id, false) { }
        }

        [TestMethod]
        public void Should_ThrowWhenIdIsDefault()
        {
            var exception = Assert.ThrowsException<ArgumentException>(() => new TestEntity(Guid.Empty));

            Assert.AreEqual("id", exception.ParamName);
        }

        [TestMethod]
        public void Should_KeepIdAndIsDeletedFromConstructor()
        {
            var id = Guid.NewGuid();

            var entity = new TestEntity(id, isDeleted: true);

            Assert.AreEqual(id, entity.Id);
            Assert.IsTrue(entity.IsDeleted);
        }

        [TestMethod]
        public void Should_MarkAsDeleted()
        {
            var entity = new TestEntity(Guid.NewGuid());

            entity.Delete();

            Assert.IsTrue(entity.IsDeleted);
        }

        [TestMethod]
        public void Should_EqualItself()
        {
            var entity = new TestEntity(Guid.NewGuid());

            Assert.IsTrue(entity.Equals(entity));
        }

        [TestMethod]
        public void Should_NotEqualOtherInstance()
        {
            var entity = new TestEntity(Guid.NewGuid());
            var other = new TestEntity(Guid.NewGuid());

            Assert.IsFalse(entity.Equals(other));
            Assert.IsFalse(entity == other);
            Assert.IsTrue(entity != other);
        }

        [TestMethod]
        public void Should_EqualOtherInstanceWithSameId()
        {
            var id = Guid.NewGuid();

            var entity = new TestEntity(id);
            var sameId = new TestEntity(id, isDeleted: true);

            Assert.IsTrue(entity.Equals(sameId));
            Assert.IsTrue(entity == sameId);
            Assert.IsFalse(entity != sameId);
        }

        [TestMethod]
        public void Should_NotEqualOtherEntityType()
        {
            var id = Guid.NewGuid();

            Assert.IsFalse(new TestEntity(id).Equals(new OtherEntity(id)));
        }

        [TestMethod]
        public void Should_NotEqualOtherType()
        {
            var entity = new TestEntity(Guid.NewGuid());

            Assert.IsFalse(entity.Equals("not an entity"));
            Assert.IsFalse(entity.Equals(null));
        }

        [TestMethod]
        public void Should_CompareNullsWithOperators()
        {
            TestEntity left = null;
            TestEntity right = null;
            var entity = new TestEntity(Guid.NewGuid());

            Assert.IsTrue(left == right);
            Assert.IsFalse(left != right);
            Assert.IsFalse(left == entity);
            Assert.IsFalse(entity == right);
            Assert.IsTrue(entity != right);
        }

        [TestMethod]
        public void Should_HashByTypeAndId()
        {
            var id = Guid.NewGuid();

            var entity = new TestEntity(id);
            var sameId = new TestEntity(id);

            Assert.AreEqual(entity.GetHashCode(), sameId.GetHashCode());
            Assert.AreNotEqual(entity.GetHashCode(), new TestEntity(Guid.NewGuid()).GetHashCode());
        }

        [TestMethod]
        public void Should_DescribeItselfWithTypeAndId()
        {
            var id = Guid.NewGuid();

            var entity = new TestEntity(id);

            Assert.AreEqual($"TestEntity [Id={id}]", entity.ToString());
        }
    }
}
