using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn
{  
    public class NamingRule : WriteRuleReport
    {
        int FDSF;
        string[] csFilesList;
        string projectPath;
        List<string> nonNamingRule;

        public NamingRule(string[] csFilesList, string projectPath)
        {
            this.csFilesList = csFilesList;
            this.projectPath = projectPath;
            this.nonNamingRule = new List<string>();
        }

        public List<string> AnalyzeNamingRule()
        {
            foreach (var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                //Pascal And Camel
                AnayzePascalAndCamel(root, csFile);

                //상수
                AnalyzeConstantUpperSnake(root, csFile);

                //인터페이스
                AnalyzeInterfaceRule(root, csFile);
            }                 
            return nonNamingRule;
        }

        void AnayzePascalAndCamel(SyntaxNode root, string csFile)
        {
            foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var className = classDeclaration.Identifier.ValueText;
                if (!IsPascalCase(className))
                {
                    int lineNum = classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    nonNamingRule.Add(WriteNonePascalClass(className, csFile, lineNum));
                }

                foreach (var member in classDeclaration.Members)
                {
                    if (member is MethodDeclarationSyntax method)
                    {
                        var methodName = method.Identifier.ValueText;
                        if (!IsPascalCase(methodName))
                        {
                            int lineNum = method.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                            nonNamingRule.Add(WriteNonePascalMethod(methodName, className, csFile, lineNum));
                        }
                        AnalyzeParameterCamel(method, csFile, className);
                    }
                    else if (member is PropertyDeclarationSyntax property)
                    {
                        var propertyName = property.Identifier.ValueText;
                        if (!IsPascalCase(propertyName))
                        {
                            int lineNum = property.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                            nonNamingRule.Add(WriteNonePascalProperty(propertyName, className, csFile, lineNum));
                        }
                    }
                    else if (member is FieldDeclarationSyntax field)
                    {
                        if (!field.Modifiers.Any(SyntaxKind.ConstKeyword))
                        {
                            AnalyzeVariableCamel(field, csFile, className);
                        }
                    }
                }
            }
        }

        void AnalyzeParameterCamel(MethodDeclarationSyntax method, string csFile, string className)
        {
            foreach (var parameter in method.ParameterList.Parameters)
            {
                var parameterName = parameter.Identifier.ValueText;
                if (!IsCamelCase(parameterName))
                {
                    int lineNum = parameter.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    nonNamingRule.Add(WriteNoneCamelParameter(parameterName, method.Identifier.ValueText, className, csFile, lineNum));
                }
            }
        }

        void AnalyzeVariableCamel(FieldDeclarationSyntax field, string csFile, string className)
        {
            foreach (var variable in field.Declaration.Variables)
            {
                var fieldName = variable.Identifier.ValueText;
                if (!IsCamelCase(fieldName))
                {
                    int lineNum = field.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    nonNamingRule.Add(WriteNoneCamelVariable(fieldName, className, csFile,lineNum));
                }
            }
        }

        void AnalyzeConstantUpperSnake(SyntaxNode root, string csFile)
        {
            foreach (var constDeclaration in root.DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                if (constDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    foreach (var variable in constDeclaration.Declaration.Variables)
                    {
                        var constantName = variable.Identifier.ValueText;
                        if (!IsUpperSnakeCase(constantName))
                        {
                            int lineNum = constDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                            nonNamingRule.Add(WriteNoneUpperSnakeCaseConstant(constantName, csFile, lineNum));
                        }
                    }
                }
            }
        }
        void AnalyzeInterfaceRule(SyntaxNode root, string csFile)
        {
            foreach (var interfaceDeclaration in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>())
            {
                var interfaceName = interfaceDeclaration.Identifier.ValueText;
                if (!IsInterfaceNameValid(interfaceName))
                {
                    int lineNum = interfaceDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    nonNamingRule.Add(WriteNoneValidInterfaceName(interfaceName, csFile, lineNum));
                }
            }
        }

        static bool IsPascalCase(string s)
        {
            return !string.IsNullOrEmpty(s) && char.IsUpper(s[0]);
        }

        bool IsCamelCase(string s)
        {
            return !string.IsNullOrEmpty(s) && char.IsLower(s[0]) && s.All(char.IsLetterOrDigit);
        }

        bool IsUpperSnakeCase(string s)
        {
            return !string.IsNullOrEmpty(s) && s.All(c => c == '_' || char.IsUpper(c) || char.IsDigit(c));
        }

        bool IsInterfaceNameValid(string s)
        {
            return s.StartsWith("I", StringComparison.Ordinal);
        }       
    }

   
}
