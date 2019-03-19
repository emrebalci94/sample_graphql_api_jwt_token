using GraphQL.Types;
using sample_graphql_api.Models;

namespace sample_graphql_api.Graphql.Types
{
    public class BrandType : ObjectGraphType<Brand>
    {
        public BrandType()
        {
            Name = "Brand";
            Field(p => p.Id);
            Field(p => p.Name);
        }
    }
}