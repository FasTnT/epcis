﻿using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingLT_INNER_ILMDParameterWithDateTimeValue : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "LT_INNER_ILMD_namespace#ilmd", Values = new[] { "2019-08-24T10:05:02Z" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSourceDestinationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<ComparisonCustomFieldFilter>(f => f.IsInner == true && f.Comparator == FilterComparator.LessThan && f.Field.Namespace == "namespace" && f.Field.Name == "ilmd" && f.Field.Type == FieldType.Ilmd && (DateTime)f.Value == new DateTime(2019, 08, 24, 10, 05, 02, DateTimeKind.Utc))), Times.Once);
        }
    }
}
