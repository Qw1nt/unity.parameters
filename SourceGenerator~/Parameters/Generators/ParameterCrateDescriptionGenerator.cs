using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Parameters.Initializer.Generator.Base;


namespace Parameters.Initializer.Generator.Generators;

[Generator]
public class ParameterCrateDescriptionGenerator : IIncrementalGenerator
{
    private readonly ParameterTypeGeneratorValidator _syntaxReceiver = new();
    private readonly StringBuilder _builder = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var itemsToGeneration = context.SyntaxProvider
            .CreateSyntaxProvider(_syntaxReceiver.IsValidType, _syntaxReceiver.IsValidDeclaration)
            .Where(x => x is { SemanticModel: not null, Declaration: not null })
            .Collect();

        context.RegisterSourceOutput(itemsToGeneration, Execute);
    }

    private void Execute(SourceProductionContext context,
        ImmutableArray<ItemToGeneration<ClassDeclarationSyntax>> source)
    {
        const string baseClassName = "Parameter";

        foreach (var syntax in source)
        {
            var declarationSymbol = ModelExtensions.GetDeclaredSymbol(syntax.SemanticModel, syntax.Declaration);
            var type = syntax.SemanticModel.Compilation!.GetTypeByMetadataName(declarationSymbol!.ContainingNamespace +
                                                                               "." + declarationSymbol.MetadataName)!;

            _builder.Clear();

            var typeName = type.Name;
            var attribute = type
                .GetAttributes()
                .First(x => x.AttributeClass?.Name == "ParameterAttribute" && x.ConstructorArguments.Length == 1);

            
            var generatedType = attribute.ConstructorArguments[0].Value;

            var parameterExtensionName = typeName.Replace("Crate", "Parameter");
            var getParameterExtensionName = "Get" + typeName.Replace("Crate", "Parameter");

            var text = $$"""
                         using Qw1nt.SelfIds.Runtime;
                         using Parameters.Runtime.Base;
                         using Parameters.Runtime.Common;
                         using Parameters.Runtime.Interfaces;
                         using Unity.IL2CPP.CompilerServices;
                         using Scellecs.Collections;
                         using System;
                         using System.Collections.Generic;
                         using System.Runtime.CompilerServices;
                         
                         namespace {{type.ContainingNamespace}}
                         {
                             [Serializable]
                             public partial class {{typeName}} : {{baseClassName}}
                             {
                                 internal static StaticId StaticId { get; set; }
                             
                                 internal {{typeName}}(ulong id, ParameterDocker docker) : base(id, docker)
                                 {
                                 }
                                 
                                 public override void SetStaticId(ulong id)
                                 {
                                     StaticId = new StaticId(id);
                                 }
                         
                                 public override Parameter CreateInstance(float rawValue = 0f, float rawOverallValue = 1f)
                                 {
                         #if UNITY_EDITOR
                                     throw new Exception();
                         #endif
                         
                                     return null;
                                 }
                         
                                 public override Parameter CreateInstance(Id id, CrateType type, ParameterDocker docker)
                                 {
                                     return null;
                                 }
                             }
                         """;

            _builder.Append(text);
            
            _builder.Append($$"""
                              
                                          
                                  public static partial class ParameterDockerExtensions
                                  {
                                      [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                      public static bool TryGet{{typeName}}(this ParameterDocker docker, out {{typeName}} result)
                                      {
                                          var id = {{typeName}}.StaticId.Value;
                                          result = null;
                                          
                                          if (docker.HasCrate(id) == false)
                                             return false;
                                          
                                          result = ({{typeName}})docker.GetParameter(id);
                                          return true;
                                      }     
                                      
                                      [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                      public static {{typeName}} Get{{typeName}}(this ParameterDocker docker)
                                      {
                                          return ({{typeName}})docker.GetParameter({{typeName}}.StaticId.Value);
                                      }     
                                      
                                      [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                      public static {{typeName}}.Parameter Create{{parameterExtensionName}}(this ParameterDocker docker)
                                      {
                                          var crate = docker.GetCrate({{typeName}}.Parameter.Id.Value);
                                      
                              #if UNITY_EDITOR
                                          if (crate is ParameterCrateDescriptionBase == false)
                                              throw new ArgumentException("");
                              #endif
                              
                                          if (crate is ParameterCrateDescriptionBase parameterCrate == true)
                                              return ({{typeName}}.Parameter) parameterCrate.CreateParameter();
                              
                                          return null;
                                      }
                                      
                                      public static {{typeName}}.Parameter {{getParameterExtensionName}}(this FastList<IParameterRef> parameters)
                                      {
                                          foreach (var parameter in parameters)
                                          {
                                              if (parameter.ParameterId == {{typeName}}.Parameter.Id.Value)
                                                  return ({{typeName}}.Parameter) parameter;
                                          }
                                          
                                          return null;
                                      }
                                  }
                              """);

            _builder.Append("\n}");
            context.AddSource(typeName + ".Default.g.cs", SourceText.From(_builder.ToString(), Encoding.UTF8));
        }
    }
}

public class ParameterTypeGeneratorValidator : SyntaxReceiverBase<ClassDeclarationSyntax>
{
    public ParameterTypeGeneratorValidator() : base("Parameter")
    {
    }

    protected override bool ValidateAttributeApplier(ISymbol declarationSymbol, INamedTypeSymbol type,
        GeneratorSyntaxContext context)
    {
        return true;
    }
}