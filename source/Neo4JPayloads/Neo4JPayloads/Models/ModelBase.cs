using System.Collections.Generic;

namespace Neo4JPayloads.Models
{
    public class ModelBase
    {
        public string Id { get; set; }

        public IEnumerable<string> Labels { get; set; }
    }
}