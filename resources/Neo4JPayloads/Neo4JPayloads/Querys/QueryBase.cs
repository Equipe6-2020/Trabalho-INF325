using System.Text;
using Neo4JPayloads.Models;

namespace Neo4JPayloads.Querys
{
    public abstract class QueryBase
    {
        protected static string GetLabels(ModelBase model)
        {
            var sb = new StringBuilder().Append(":" + model.GetType().Name);

            if (model.Labels == null) return sb.ToString();
            foreach (var label in model.Labels)
                sb.Append(":" + label);

            return sb.ToString();
        }
    }
}