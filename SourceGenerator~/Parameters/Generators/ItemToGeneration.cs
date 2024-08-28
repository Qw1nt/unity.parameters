using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Parameters.Initializer.Generator.Generators;

public readonly record struct ItemToGeneration<TDeclaration>(TDeclaration Declaration, SemanticModel SemanticModel)
    where TDeclaration : TypeDeclarationSyntax
{
    public TDeclaration Declaration { get; } = Declaration;

    public SemanticModel SemanticModel { get; } = SemanticModel;
}