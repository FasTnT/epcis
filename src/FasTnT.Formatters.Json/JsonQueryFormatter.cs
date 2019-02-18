using System.IO;
using FasTnT.Model.Queries;

namespace FasTnT.Formatters.Json
{
    public class JsonQueryFormatter : IQueryFormatter
    {
        public EpcisQuery Read(Stream input)
        {
            throw new System.NotImplementedException();
        }

        public void Write(EpcisQuery entity, Stream output)
        {
            throw new System.NotImplementedException();
        }
    }
}
