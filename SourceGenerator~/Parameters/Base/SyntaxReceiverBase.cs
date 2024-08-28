using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Parameters.Initializer.Generator.Generators;

namespace Parameters.Initializer.Generator.Base
{
    public abstract class SyntaxReceiverBase<TDeclaration>
        where TDeclaration : TypeDeclarationSyntax
    {
        private readonly List<string> _availableNames;

        protected SyntaxReceiverBase(string attributeName, string attributeNamespace = "Parameters.Runtime.Attributes.")
        {
            var attributeFullname = attributeName + "Attribute";

            _availableNames = new List<string>
            {
                attributeName,
                attributeFullname,
                attributeNamespace + attributeName,
                attributeNamespace + attributeFullname
            };
        }

        public bool IsValidType(SyntaxNode node, CancellationToken arg2)
        {
            return node is TDeclaration {AttributeLists.Count: > 0};
        }

        public ItemToGeneration<TDeclaration> IsValidDeclaration(GeneratorSyntaxContext context, CancellationToken arg2)
        {
            var declaration = (TDeclaration) context.Node;

            foreach (var attributeList in declaration.AttributeLists)
            {
                foreach (var attributeSyntax in attributeList.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not { } attributeSymbol)
                        continue;

                    var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    var fullName = attributeContainingTypeSymbol.ToDisplayString();

                    if (_availableNames.Contains(fullName) == false)
                        continue;
                    
                    var declarationSymbol = context.SemanticModel.GetDeclaredSymbol(declaration);
                    var type = context.SemanticModel.Compilation!.GetTypeByMetadataName(
                        declarationSymbol!.ContainingNamespace + "." + declarationSymbol.MetadataName)!;

                    if(ValidateAttributeApplier(declarationSymbol, type ,context) == false)
                        continue;

                    return new ItemToGeneration<TDeclaration>(declaration, context.SemanticModel);
                }
            }

            return new ItemToGeneration<TDeclaration>();
        }

        protected abstract bool ValidateAttributeApplier(ISymbol declarationSymbol, INamedTypeSymbol type, GeneratorSyntaxContext context);
    }
}