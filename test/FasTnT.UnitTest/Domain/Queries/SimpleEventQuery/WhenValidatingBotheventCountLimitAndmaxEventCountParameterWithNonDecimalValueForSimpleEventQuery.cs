﻿using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.Queries
{
    [TestClass]
    public class WhenValidatingBotheventCountLimitAndmaxEventCountParameterWithNonDecimalValueForSimpleEventQuery : SimpleEventQueryParameterValidationFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Parameters = new QueryParameter[]
            {
                new QueryParameter{ Name = "eventCountLimit", Values = new []{ "10" } },
                new QueryParameter{ Name = "maxEventCount", Values = new []{ "10" } }
            };
        }

        [Assert]
        public void ItShouldThrowAnException() => Assert.IsNotNull(Catched);

        [Assert]
        public void TheExceptionShouldBeEpcisException() => Assert.IsInstanceOfType(Catched, typeof(EpcisException));
    }
}