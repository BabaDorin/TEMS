using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace temsAPI.Testing
{
    public enum TestLog
    {
        OnDeleteCascade
    }

    public class WriteTestLogs
    {
        private static readonly string onDeleteCascadeLogFilePath = @"Testing\onDeleteCascadeTestingLogs.txt";

        public static  void WriteTestLog(TestLog testLog, bool emptyFileBeforeWritting, List<string> logs)
        {
            switch (testLog)
            {
                case TestLog.OnDeleteCascade:
                    WriteLog(onDeleteCascadeLogFilePath, emptyFileBeforeWritting, logs);
                    break;
            }
        }


        private static void WriteLog(string filePath, bool emptyFileBeforeWritting, List<string> logs)
        {
            using (StreamWriter sr = new StreamWriter(filePath, !emptyFileBeforeWritting))
            {
                sr.WriteLine("\n---------------------Test Case----------------------");
                logs.ForEach(q => sr.WriteLine(q));
            }
        }
    }
}
