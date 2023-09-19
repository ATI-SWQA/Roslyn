using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn
{
    public class LimitRule : WriteRuleReport
    {
        private static readonly int MaxMethodLength = 50; // 원하는 최대 메서드 길이
        string[] csFilesList;
        string projectPath;
        List<string> noneLimitRule;

        public LimitRule(string[] csFilesList, string projectPath)
        {
            this.csFilesList = csFilesList;
            this.projectPath = projectPath;
            this.noneLimitRule = new List<string>();
        }

        public List<string> AnalyzeLimitRule()
        {
            foreach(var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                AnayzeMethodLengh(root, csFile);
            }

            return noneLimitRule;
        }

        void AnayzeMethodLengh(SyntaxNode root, string csFile)
        {
            var methodNodes = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var methodNode in methodNodes)
            {
                var classNode = methodNode.Parent as ClassDeclarationSyntax;
                var methodLength = GetMethodLength(methodNode);
                if (methodLength > MaxMethodLength)
                {
                    int lineNum = methodNode.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    var methodName = methodNode.Identifier.ValueText;
                    if (classNode != null)
                    {
                        var className = classNode.Identifier.ValueText;
                        noneLimitRule.Add(WriteMethodLengthLimit(methodName, className, csFile, lineNum));
                    }
                    else
                    {
                        noneLimitRule.Add(WriteMethodLengthLimit(methodName, " not contained in a class", csFile, lineNum));
                    }
                }
            }
        }

        private int GetMethodLength(MethodDeclarationSyntax method)
        {
            var body = method.Body;
            if (body == null)
                return 0;

            var lines = body.GetText().Lines;
            return lines.Count;
        }
    }
}
