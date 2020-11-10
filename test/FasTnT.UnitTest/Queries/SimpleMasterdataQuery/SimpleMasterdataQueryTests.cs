using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    [TestClass]
    public class SimpleMasterdataQueryTests
    {
        [TestMethod]
        public void ItShouldNotCallAnyApplyMethod()
        {
            var query = new Domain.Queries.SimpleMasterdataQuery(default);
            var isSubscriptionAllowed = query.AllowSubscription;

            Assert.IsFalse(isSubscriptionAllowed);
        }
    }
}
