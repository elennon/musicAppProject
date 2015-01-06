using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace MyMusic.HelperClasses
{
    public sealed class Log
    {
        private StorageFolder logFolder;
        private StringBuilder logSb;
        static private Log log;

        static public Log GetLog()
        {
            if (log == null)
            {
                log = new Log();
            }
            return log;
        }

        public void InitiateLog()
        {
            logSb = new StringBuilder();
        }

        static public bool CheckIfNull()
        {
            if (log == null)
            {
                return true;
            }
            else
                return false;
        }

        public void Write(string tevxt)
        {
            logSb.AppendLine(tevxt + Environment.NewLine);
        }

        public async void SaveLogFile()
        {
            logFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("LogFile", CreationCollisionOption.OpenIfExists);
            var filename = DateTime.Now.ToString("Log-yyyyMMdd") + ".txt";
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(logSb.ToString().ToCharArray());

            StorageFile file = await logFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.Write(fileBytes, 0, fileBytes.Length);
            }
        }
    }
}
