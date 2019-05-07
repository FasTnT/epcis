using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.Queries
{
    [TestClass]
    public class WhenExecutingSimpleEventQueryWithNullParameters : SimpleEventQueryFixture
    {
        public IEnumerable<IEntity> Result { get; private set; }

        public override void Act()
        {
            Result = Query.Execute(null, UnitOfWork, default).Result;
        }

        [Assert]
        public void ItShouldReturnNull() => Assert.AreEqual(0, Result.Count());
    }
}
