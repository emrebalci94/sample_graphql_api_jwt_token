using GraphQL;
using GraphQL.Types;

namespace sample_graphql_api.Graphql
{

    public class MarketingSchema : Schema
    {
        public MarketingSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<MarketingQuery>();
        }
    }
}