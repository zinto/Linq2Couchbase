﻿using System.Linq;
using Couchbase.Core;
using Couchbase.Linq.Extensions;
using Couchbase.Linq.Tests.Documents;
using Moq;
using NUnit.Framework;

namespace Couchbase.Linq.Tests.QueryGeneration
{
    [TestFixture]
    public class WhereClauseTests : N1QLTestBase
    {
        [Test]
        public void Test_Where_With_OrderBy()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Contact>(mockBucket.Object)
                    .Where(e => e.Age > 10 && e.FirstName == "Sam")
                    .OrderBy(e => e.Age)
                    .Select(e => new {age = e.Age, name = e.FirstName});

            const string expected =
                "SELECT e.age as age, e.fname as name FROM default as e WHERE ((e.age > 10) AND (e.fname = 'Sam')) ORDER BY e.age ASC";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        [Test]
        public void Test_Where_With_Missing()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Contact>(mockBucket.Object)
                    .Where(e => e.Email == "something@gmail.com")
                    .WhereMissing(g => g.Age)
                    .OrderBy(e => e.Age)
                    .Select(e => new {age = e.Age, name = e.FirstName});

            const string expected =
                "SELECT e.age as age, e.fname as name FROM default as e WHERE (e.email = 'something@gmail.com') AND e.age IS MISSING ORDER BY e.age ASC";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        [Test]
        public void Test_Where_With_Like()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Contact>(mockBucket.Object)
                    .Where(e => e.Age > 10 && e.FirstName == "Sam" && e.LastName.Contains("a"))
                    .Select(e => new {age = e.Age, name = e.FirstName});

            const string expected =
                "SELECT e.age as age, e.fname as name FROM default as e WHERE (((e.age > 10) AND (e.fname = 'Sam')) AND (e.lname LIKE '%a%'))";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        [Test]
        public void Test_Where()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Contact>(mockBucket.Object)
                    .Where(e => e.Age > 10 && e.FirstName == "Sam")
                    .Select(e => new {age = e.Age, name = e.FirstName});


            const string expected =
                "SELECT e.age as age, e.fname as name FROM default as e WHERE ((e.age > 10) AND (e.fname = 'Sam'))";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        [Test]
        public void Test_Where_With_Parameters()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            const int age = 10;
            const string firstName = "Sam";

            var query =
                QueryFactory.Queryable<Contact>(mockBucket.Object)
                    .Where(e => e.Age > age && e.FirstName == firstName)
                    .Select(e => new {age = e.Age, name = e.FirstName});


            const string expected =
                "SELECT e.age as age, e.fname as name FROM default as e WHERE ((e.age > 10) AND (e.fname = 'Sam'))";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        [Test]
        public void Test_Multiple_Where()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Contact>(mockBucket.Object)
                    .Where(e => e.Age > 10 && e.FirstName == "Sam")
                    .Where(e => e.Email == "myemail@test.com")
                    .Select(e => new {age = e.Age, name = e.FirstName});

            const string expected =
                "SELECT e.age as age, e.fname as name FROM default as e WHERE ((e.age > 10) AND (e.fname = 'Sam')) AND (e.email = 'myemail@test.com')";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        [Test]
        public void Test_Where_With_Or()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Contact>(mockBucket.Object)
                    .Where(e => e.Age > 10 || e.FirstName == "Sam")
                    .Select(e => new {age = e.Age, name = e.FirstName});

            const string expected =
                "SELECT e.age as age, e.fname as name FROM default as e WHERE ((e.age > 10) OR (e.fname = 'Sam'))";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        [Test]
        public void Test_Where_With_Math()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Contact>(mockBucket.Object)
                    .Where(e => e.Age < 10 + 30)
                    .Select(e => new {age = e.Age, name = e.FirstName});

            const string expected = "SELECT e.age as age, e.fname as name FROM default as e WHERE (e.age < 40)";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }
    }
}