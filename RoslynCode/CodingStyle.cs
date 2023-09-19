using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Roslyn
{
    public class Member
    {
        public string MemberName { get; set; }
        public string MemberType { get; set; }
        public int LineNumber { get; set; }

        public Member(string memberName, string memberType, int lineNum)
        {
            this.MemberName = memberName;
            this.MemberType = memberType;
            this.LineNumber = lineNum;
        }
    }

    public class CodingStyle : WriteRuleReport
    {
        string[] csFilesList;
        string projectPath;
        List<string> noneCodingStlye;

        public CodingStyle(string[] csFilesList, string projectPath)
        {
            this.csFilesList = csFilesList;
            this.projectPath = projectPath;
            this.noneCodingStlye = new List<string>();
        }

        public List<string> AnalyzeCodingStyle()
        {
            foreach (var csFile in csFilesList)
            {
                var fullPath = Path.Combine(projectPath, csFile);
                var code = File.ReadAllText(fullPath);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                AnalyzeStringConcatenation(root, csFile);
                AnalyzeMemberOrder(root, csFile);
            }

            return noneCodingStlye;
        }

        void AnalyzeStringConcatenation(SyntaxNode root, string csFile)
        {
            var Variables = new HashSet<string>();

            foreach (var declaration in root.DescendantNodes().OfType<VariableDeclarationSyntax>())
            {
                var variableType = declaration.Type as PredefinedTypeSyntax;
                if (variableType != null && variableType.Keyword.Text == "string")
                {
                    foreach (var variable in declaration.Variables)
                    {
                        Variables.Add(variable.Identifier.Text);
                    }
                }
            }

            foreach (var binaryExpression in root.DescendantNodes().OfType<BinaryExpressionSyntax>())
            {
                var leftIdentifier = binaryExpression.Left as IdentifierNameSyntax;
                var rightIdentifier = binaryExpression.Right as IdentifierNameSyntax;

                if (leftIdentifier != null && Variables.Contains(leftIdentifier.Identifier.Text) || rightIdentifier != null && Variables.Contains(rightIdentifier.Identifier.Text))
                {
                    int lineNum = binaryExpression.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    noneCodingStlye.Add(WriteRuleReport.WriteStringConcatenationIssue(csFile, lineNum));
                }
            }
        }

        void AnalyzeMemberOrder(SyntaxNode root, string csFile)
        {
            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classDeclaration in classDeclarations)
            {
                var members = classDeclaration.Members;

                Dictionary<string, int> memberTypeIndexMap = new Dictionary<string, int>
                {
                    {"Constant", 0},
                    {"Field", 1},
                    {"Property", 2},
                    {"Constructor", 3},
                    {"Method", 4},
                    {"PrivateMethod", 5},
                    {"NestedClass", 6}
                };

                // 클래스 내의 멤버 변수 목록 초기화
                var memberList = CreateMemberList(members, csFile);

                // 순서 어긋난 멤버 변수 찾기
                var unorderedMembers = GetUnorderedMembers(memberList, memberTypeIndexMap);

                var className = classDeclaration.Identifier.ValueText;
                foreach (var unorderedMember in unorderedMembers)
                {
                    noneCodingStlye.Add(WriteMemberOrderIssue(unorderedMember.MemberName, csFile, className, unorderedMember.LineNumber));
                }
            }
        }

        private List<Member> CreateMemberList(SyntaxList<MemberDeclarationSyntax> members, string csFile)
        {
            // 클래스 내의 멤버 변수 목록 초기화
            var memberList = new List<Member>();

            // 멤버 변수 순서를 검사하고 memberList에 추가
            foreach (var member in members)
            {
                if (member is FieldDeclarationSyntax fieldDeclaration)
                {
                    // 변수 타입을 얻어옵니다.
                    var memberName = fieldDeclaration.Declaration.Variables.First().Identifier.Text;
                    var lineNum = fieldDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    memberList.Add(new Member(memberName, "Constant", lineNum));
                }
                else if (member is PropertyDeclarationSyntax propertyDeclaration)
                {
                    var propertyName = propertyDeclaration.Identifier.Text;
                    var lineNum = propertyDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    memberList.Add(new Member(propertyName, "Property", lineNum));
                }
                else if (member is ConstructorDeclarationSyntax constructorDeclaration)
                {
                    var constructorName = constructorDeclaration.Identifier.Text;
                    var lineNum = constructorDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    memberList.Add(new Member(constructorName, "Constructor", lineNum));
                }
                else if (member is MethodDeclarationSyntax methodDeclaration)
                {
                    var methodName = methodDeclaration.Identifier.Text;
                    var lineNum = methodDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    if (methodDeclaration.Modifiers.Any(modifier => modifier.Kind() == SyntaxKind.PublicKeyword))
                    {
                        memberList.Add(new Member(methodName, "Method", lineNum));
                    }
                    else
                    {
                        memberList.Add(new Member(methodName, "PrivateMethod", lineNum));
                    }
                }
                else if (member is ClassDeclarationSyntax nestedClassDeclaration)
                {
                    var nestedClassName = nestedClassDeclaration.Identifier.Text;
                    var lineNum = nestedClassDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    memberList.Add(new Member(nestedClassName, "NestedClass", lineNum));
                }
                // 상수를 추가하려면 추가적인 논리 필요
            }

            return memberList;
        }

        private List<Member> GetUnorderedMembers(List<Member> members, Dictionary<string, int> memberTypeIndexMap)
        {
            var unorderedMembers = new List<Member>();
            var currentIndex = 0;

            foreach (var member in members)
            {
                if (currentIndex < memberTypeIndexMap[member.MemberType])
                {
                    currentIndex = memberTypeIndexMap[member.MemberType];
                }
                else if (currentIndex > memberTypeIndexMap[member.MemberType])
                {
                    unorderedMembers.Add(member);
                    currentIndex = memberTypeIndexMap[member.MemberType];
                }
            }

            return unorderedMembers;
        }
    }
}