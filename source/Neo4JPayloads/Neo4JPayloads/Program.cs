using System;
using Neo4JPayloads.Business;

namespace Neo4JPayloads
{
    class Program
    {
        static void Main(string[] args) =>
            new ProcessInformationBusiness().Starter();
    }
}
