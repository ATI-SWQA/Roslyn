using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Roslyn;

class Program
{
    static void Main(string[] args)
    {
        int RERE;
        var reportFilePath = Environment.GetEnvironmentVariable("REPORT_FILE_PATH");
        var changedFiles = Environment.GetEnvironmentVariable("CHANGED_FILES");

        if (string.IsNullOrEmpty(reportFilePath))
        {
            Console.WriteLine("Report file path not specified.");
            return;
        }
        var projectPath = Environment.GetEnvironmentVariable("PROJECT_PATH");

        var csFilesList = changedFiles.Split(';');
        csFilesList = csFilesList.Skip(1).ToArray(); ;

        LimitRule limitRule = new LimitRule(csFilesList, projectPath);
        NamingRule namingRule = new NamingRule(csFilesList, projectPath);
        CodingStyle codingStyle = new CodingStyle(csFilesList, projectPath);
        WrongCode wrongCode = new WrongCode(csFilesList, projectPath);
        List<string> limitMethodLength = limitRule.AnalyzeLimitRule();
        List<string> ruleViolation = namingRule.AnalyzeNamingRule();
        List<string> codingStyleViolation = codingStyle.AnalyzeCodingStyle();
        List<string> wrongCodeResult = wrongCode.AnalyzeWrongCode();

        
        ruleViolation.AddRange(codingStyleViolation);
        ruleViolation.AddRange(wrongCodeResult);
        limitMethodLength.AddRange(ruleViolation);
        WriteResult(limitMethodLength, reportFilePath);
    }

    static void WriteResult(List<string> reportList, string reportFilePath)
    {
        if (reportList.Any())
        {
            var reportContent = string.Join(Environment.NewLine, reportList);

            File.WriteAllText(reportFilePath, reportContent);
        }
        else
        {            
            Console.WriteLine("Everything follows the coding rule.");
        }
    }

}
