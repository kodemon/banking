using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Banking.Principal.OpenApi;

/*
 |--------------------------------------------------------------------------------
 | PrincipalDocumentTransformer
 |--------------------------------------------------------------------------------
 |
 | Registers the Principal domain tag in the OpenAPI document.
 | Scalar uses tags to group and describe endpoints in the UI.
 |
 */

internal class PrincipalDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Tags ??= new HashSet<OpenApiTag>();

        document.Tags.Add(new OpenApiTag
        {
            Name = "Principal",
            Description = "Manage security principals — IDP bindings, roles, and domain-scoped access control attributes."
        });

        return Task.CompletedTask;
    }
}