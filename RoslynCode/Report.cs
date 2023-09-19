using System;
using System.IO;


namespace Roslyn
{
    public class WriteRuleReport
    {
        static public string WriteNonePascalClass(string className, string csFile, int lineNum)
        {
            string nonePascalClass = $"Pascal Rule (Class){Environment.NewLine}" +
                        $"Class name: {className}{Environment.NewLine}" +
                        $"Line number: {lineNum}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +                        
                        $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return nonePascalClass;
        }

        static public string WriteNonePascalMethod(string methodName, string className, string csFile, int lineNum)
        {

            string nonePascalMethod = $"Pascal Rule (Method){Environment.NewLine}" +
                                $"Method name: {methodName}{Environment.NewLine}" +
                                $"Line number: {lineNum}{Environment.NewLine}" +                                
                                $"Class name: {className}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return nonePascalMethod;
        }

        static public string WriteNonePascalProperty(string propertyName, string className, string csFile, int lineNum)
        {

            string nonePascalProperty = $"Pascal Rule (Property){Environment.NewLine}" +
                                $"Property name: {propertyName}{Environment.NewLine}" +
                                $"Line number: {lineNum}{Environment.NewLine}" +                                
                                $"Class name: {className}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return nonePascalProperty;
        }

        static public string WriteNoneCamelParameter(string parameterName, string method, string className, string csFile, int lineNum)
        {
            string noneCamelParameter = $"Camel Rule (Parameter){Environment.NewLine}" +
                        $"Parameter name: {parameterName}{Environment.NewLine}" +
                        $"Line number: {lineNum}{Environment.NewLine}" +
                        $"Method name: {method}{Environment.NewLine}" +
                        $"Class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +                        
                        $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return noneCamelParameter;
        }

        static public string WriteNoneCamelVariable(string fieldName, string className, string csFile, int lineNum)
        {
            string noneCamelVariable = $"Camel Rule (Field){Environment.NewLine}" +
                        $"Field name: {fieldName}{Environment.NewLine}" +
                        $"Line number: {lineNum}{Environment.NewLine}" +
                        $"Class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +                        
                        $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return noneCamelVariable;
        }

        static public string WriteNoneUpperSnakeCaseConstant(string constantName, string csFile, int lineNum)
        {
            string noneUpperSnakeCaseConstant = $"UpperSnakeCase Rule{Environment.NewLine}" +
                        $"Constant name: {constantName}{Environment.NewLine}" +
                        $"Line number: {lineNum}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +                        
                        $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return noneUpperSnakeCaseConstant;
        }

        static public string WriteNoneValidInterfaceName(string interfaceName, string csFile, int lineNum)
        {
            string noneValidInterfaceName = $"Interface Name Rule{Environment.NewLine}" +
                        $"Interface name: {interfaceName}{Environment.NewLine}" +
                        $"Line number: {lineNum}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +                        
                        $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return noneValidInterfaceName;
        }

        static public string WriteStringConcatenationIssue(string csFile, int lineNum)
        {
            string stringConcatenationIssue = $"String Concatenation Rule{Environment.NewLine}" +
                       $"Line number: {lineNum}{Environment.NewLine}" +
                       $"File name: {csFile}{Environment.NewLine}" +                       
                       $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return stringConcatenationIssue;       
        }

        static public string WriteMemberOrderIssue(string memberName, string csFile, string className, int lineNum)
        {
            string unorderedMember = $"Unordered Member Rule{Environment.NewLine}" +
                        $"Member name: {memberName}{Environment.NewLine}" +
                        $"Line number: {lineNum}{Environment.NewLine}" +
                        $"Class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +
                        $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return unorderedMember;
        }

        static public string WriteUnusedMember(string memberName, string declaredMember, string className, string csFile, int lineNum)
        {
            string unusedMember = $"Unused Member{Environment.NewLine}" +
                        $"Member name: {memberName}{Environment.NewLine}" +
                        $"Member type: {declaredMember}{Environment.NewLine}" +
                        $"Line number: {lineNum}{Environment.NewLine}" +
                        $"Class name: {className}{Environment.NewLine}" +
                        $"File name: {csFile}{Environment.NewLine}" +                        
                        $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return unusedMember;
        }

        static public string WriteMethodLengthLimit(string methodName, string className, string csFile, int lineNum)
        {

            string noneLimitRule = $"Method Length Limit (Method){Environment.NewLine}" +
                                $"Method name: {methodName}{Environment.NewLine}" +
                                $"Line number: {lineNum}{Environment.NewLine}" +
                                $"Class name: {className}{Environment.NewLine}" +
                                $"File name: {csFile}{Environment.NewLine}" +
                                $"Project name: {Path.GetFileNameWithoutExtension(csFile)}{Environment.NewLine}";

            return noneLimitRule;
        }
    }
}
