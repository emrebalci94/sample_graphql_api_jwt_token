using GraphQL.Types;
using sample_graphql_api.Models;

namespace sample_graphql_api.Graphql.Types
{
    public class MaterialType : ObjectGraphType<Material>
    {
        public MaterialType()
        {
            Name = "Material";
            Field(p => p.Id);
            Field(p => p.Name);
            Field(p => p.Piece);
            Field<BrandType>("Brand", resolve: _ => _.Source.Brand);
        }
    }

}