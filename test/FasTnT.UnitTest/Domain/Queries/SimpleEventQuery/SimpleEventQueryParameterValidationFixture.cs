using System;
using System.Collections.Generic;
using FasTnT.Model.Queries;

namespace FasTnT.UnitTest.Domain.Queries
{
    public abstract class SimpleEventQueryParameterValidationFixture : SimpleEventQueryFixture
    {
        public Exception Catched { get; set; }
        public IEnumerable<QueryParameter> Parameters { get; set; }

        public override void Act()
        {
            try
            {
                Query.ValidateParameters(Parameters);
            }
            catch (Exception ex)
            {
                Catched = ex;
            }
        }
    }
}
