using GraphQL;
using GraphQL.Types;
using sample_graphql_api.Graphql.Types;
using sample_graphql_api.Helpers;

namespace sample_graphql_api.Graphql
{
    public class MarketingQuery : ObjectGraphType<object>
    {
        public MarketingQuery(MarketingContext _marketingContext)
        {
            Name = "Marketing_Query";
            //TODO: Graphql endpointlerini belirliyoruz.
            Field<ListGraphType<MaterialType>>("Materials",
            resolve: ctx => _marketingContext.GetMaterials());
            Field<ListGraphType<MaterialType>>("MeterialByBrandId",
            arguments: new QueryArguments
            {
             new QueryArgument<IntGraphType>{
                 Name="Id",
                 Description="Brand Id"
             }
            },
             resolve: ctx => _marketingContext.GetMaterialsByBrandId(ctx.GetArgument<int>("Id")));
            Field<ListGraphType<BrandType>>("Brands", resolve: ctx => _marketingContext.GetBrands());
        }
    }
}