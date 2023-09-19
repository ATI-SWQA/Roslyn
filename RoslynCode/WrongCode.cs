using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn
{
    class WrongCode : WriteRuleReport
    {
        string[] csFilesList;
        string projectPath;
        List<string> wrongCode;

        public WrongCode(string[] csFilesList, string projectPath)
        {
            this.csFilesList = csFilesList;
            this.projectPath = projectPath;
            this.wrongCode = new List<string>();
        }

        public List<string> AnalyzeWrongCode()
        {
            foreach (var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                AnalyzeUnusedMembers(root, csFile);
            }
            return wrongCode;
        }

        void AnalyzeUnusedMembers(SyntaxNode root, string csFile)
        {
            var declaredMembers = new List<string>
            {
                "Variable", // 변수
                "Constant", // 상수
                "Field",    // 필드
                "Property", // 속성
            };

            foreach (var declaredMember in declaredMembers)
            {
                var declaredMemberNames = root.DescendantNodes()
                    .OfType<MemberDeclarationSyntax>()
                    .Where(member => GetMemberType(member) == declaredMember)
                    .Select(member => GetMemberName(member));

                var usedMemberNames = root.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Select(identifier => identifier.Identifier.Text);

                var unusedMemberNames = declaredMemberNames.Except(usedMemberNames);

                foreach (var memberName in unusedMemberNames)
                {
                    var className = Path.GetFileNameWithoutExtension(csFile);
                    var lineNum = GetLineNumberOfMember(root, memberName);
                    wrongCode.Add(WriteUnusedMember(memberName, declaredMember, className, csFile, lineNum));
                }
            }
        }

        private string GetMemberType(MemberDeclarationSyntax member)
        {
            if (member is FieldDeclarationSyntax)
                return "Field";
            if (member is PropertyDeclarationSyntax)
                return "Property";

            return "";
        }

        private string GetMemberName(MemberDeclarationSyntax member)
        {
            if (member is FieldDeclarationSyntax fieldDeclaration)
            {
                var variable = fieldDeclaration.Declaration.Variables.FirstOrDefault();
                return variable != null ? variable.Identifier.Text : "";
            }
            if (member is PropertyDeclarationSyntax propertyDeclaration)
                return propertyDeclaration.Identifier.Text;
            if (member is ConstructorDeclarationSyntax constructorDeclaration)
                return constructorDeclaration.Identifier.Text;
            if (member is MethodDeclarationSyntax methodDeclaration)
                return methodDeclaration.Identifier.Text;

            return "";
        }

        private int GetLineNumberOfMember(SyntaxNode root, string memberName)
        {
            var memberDeclarations = root.DescendantNodes()
                .OfType<MemberDeclarationSyntax>()
                .Where(member => GetMemberName(member) == memberName);

            foreach (var declaration in memberDeclarations)
            {
                var lineSpan = declaration.GetLocation().GetLineSpan();
                if (lineSpan.IsValid)
                {
                    return lineSpan.StartLinePosition.Line + 1;
                }
            }
            return -1; // 멤버를 찾지 못한 경우
        }
    }
}
