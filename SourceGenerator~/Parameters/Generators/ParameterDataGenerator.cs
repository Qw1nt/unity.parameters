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
public class ParameterDataGenerator : IIncrementalGenerator
{
    private readonly ParameterDataGeneratorValidator _syntaxReceiver = new();
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
        ImmutableArray<ItemToGeneration<StructDeclarationSyntax>> items)
    {
        const string parameterRefClassName = "Ref";
        
        foreach (var item in items)
        {
            var declarationSymbol = item.SemanticModel.GetDeclaredSymbol(item.Declaration);
            var type = item.SemanticModel.Compilation!.GetTypeByMetadataName(declarationSymbol!.ContainingNamespace +
                                                                             "." + declarationSymbol.MetadataName)!;

            _builder.Clear();

            var typeName = type.Name;
            var attribute = type
                .GetAttributes()
                .First(x => x.ConstructorArguments.Length == 1);

            var typeDescriptor = attribute.ConstructorArguments[0].Value;
            var descriptorTypeInfo = item.SemanticModel.Compilation!.GetTypeByMetadataName(typeDescriptor!.ToString());
            var members = descriptorTypeInfo?.GetMembers() ?? new ImmutableArray<ISymbol>();

            string stashName = typeName + "Stash";

            _builder.Append($$"""
                              using System;
                              using System.Runtime.CompilerServices;

                              namespace {{type.ContainingNamespace}}
                              {
                                  public partial struct {{typeName}} : IComponent
                                  {
                                  
                              """);

            var descriptorMembers = new List<DescriptorMember>();

            if (members.Length > 1)
            {
                for (int i = 0; i < members.Length - 1; i++)
                {
                    var member = members[i] as IFieldSymbol;

                    if(member == null)
                        continue;
                    
                    descriptorMembers.Add(new DescriptorMember
                    {
                        Type = member.Type.ToString(),
                        Name = member.Name
                    });
                }
            }

            foreach (var descriptorMember in descriptorMembers)
                _builder.Append($"""    public {descriptorMember.Type} {descriptorMember.Name};""" + '\n');

            _builder.Append($$"""
                              
                                      [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                      public static {{typeDescriptor}} GetDescriptor()
                                      {
                                          return new {{typeDescriptor}}();
                                      }
                            """);
                              
            MapToModelDescriptor(typeDescriptor, descriptorTypeInfo, typeName, descriptorMembers);
            MapFromModelDescriptor(typeDescriptor, descriptorTypeInfo, typeName, descriptorMembers);

            _builder.Append($$"""

                                    
                                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                    public static {{parameterRefClassName}} CreateRef()
                                    {
                                        return new {{parameterRefClassName}}();
                                    }
                                
                                    public class {{parameterRefClassName}} : ParameterRef, IParameterRef
                                    {
                                        internal static StaticId StaticId;
                                    
                                        private {{typeName}} _value;

                                        internal {{parameterRefClassName}}()
                                        {
                                        }     
                                                                           
                                        internal {{parameterRefClassName}}(double defaultValue)
                                        {
                                            SetValue(defaultValue);
                                        }

                                        public static ulong Id 
                                        {
                                            [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                            get => StaticId.Value;
                                        }
                            
                                        public ulong ParameterId 
                                        {
                                            [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                            get => StaticId.Value;
                                        }
                                        
                                        public {{typeName}} Value 
                                        {
                                            [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                            get => _value;
                                            
                                            [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                            set 
                                            {
                                                _value = value;
                                                ChangesEmitter?.Invoke(this);
                                            }
                                        }
                                        
                                        // public bool HasChanges 
                                        // { 
                                        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                        //     get;
                                        //     
                                        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                        //     private set;
                                        // }
                                        
                                        internal override void SetId(ulong id)
                                        {
                                            StaticId = new StaticId(id);
                                        }
                                        
                                        [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                        public void SetValue(double value)
                                        {
                                            var descriptor = {{typeName}}.GetDescriptor().SetValue(value); 
                                            Value = {{typeName}}.MapFromDescriptorModel(descriptor);
                                        }
                                        
                                        [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                        internal override float GetValue()
                                        {
                                            return {{typeName}}.MapToDescriptorModel(Value);
                                        }
                                    }
                                }
                            }
                            """);

            context.AddSource(typeName + ".Data.g.cs", SourceText.From(_builder.ToString(), Encoding.UTF8));
        }
    }

    private void MapToModelDescriptor(object typeDescriptor, INamedTypeSymbol descriptorTypeInfo, string typeName, List<DescriptorMember> descriptorMembers)
    {
        _builder.Append($$"""
                          
                          
                                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                    public static {{typeDescriptor}} MapToDescriptorModel({{typeName}} parameter)
                                    {
                                        return new {{descriptorTypeInfo}}
                                        {
                                    
                          """);
        
        foreach (var descriptorMember in descriptorMembers)
            _builder.Append($"\t     {descriptorMember.Name} = parameter.{descriptorMember.Name},\n");
                        
        _builder.Append("""
                                      };
                                  }
                            
                        """);
    }
    
    private void MapFromModelDescriptor(object typeDescriptor, INamedTypeSymbol descriptorTypeInfo, string typeName, List<DescriptorMember> descriptorMembers)
    {
        _builder.Append($$"""
                          
                                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                    public static {{typeName}} MapFromDescriptorModel({{typeDescriptor}} parameter)
                                    {
                                        return new {{typeName}}
                                        {

                          """);
        
        foreach (var descriptorMember in descriptorMembers)
            _builder.Append($"\t\t{descriptorMember.Name} = parameter.{descriptorMember.Name},\n");
                        
        _builder.Append("""
                                      };
                                  }
                        """);
    }
}

public struct DescriptorMember
{
    public string Type;
    public string Name;
}

public class ParameterDataGeneratorValidator : SyntaxReceiverBase<StructDeclarationSyntax>
{
    public ParameterDataGeneratorValidator() : base("Parameter", "Scellecs.Morpeh.Parameters.Attributes.")
    {
    }

    protected override bool ValidateAttributeApplier(ISymbol declarationSymbol, INamedTypeSymbol type,
        GeneratorSyntaxContext context)
    {
        return true;
    }
}