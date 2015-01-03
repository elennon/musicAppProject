using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace MyMusic.HelperClasses
{
    public class Logger
    {

        public LoggingChannel logChannel;

        public LoggingSession logSession;

        private StorageFolder logUploadFolder;

        public const string LOG_SESSION_RESROUCE_NAME = "LogSession";

        static private Logger logger;
        private const int DAYS_TO_DELETE = 15;

        public async void InitiateLogger()
        {
            logChannel = new LoggingChannel("MySampleChannel");
            logSession = new LoggingSession("MySample Session");

            logSession.AddLoggingChannel(logChannel);

            await RegisterUnhandledErrorHandler();
        }


        /// <summary>
        /// Maintains singleton object 
        /// </summary>
        /// <returns></returns>
        static public Logger GetLogger()
        {
            if (logger == null)
            {
                logger = new Logger();
            }
            return logger;
        }


        private async Task RegisterUnhandledErrorHandler()
        {
            logUploadFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MyLogFile",
                CreationCollisionOption.OpenIfExists);

            CoreApplication.UnhandledErrorDetected += CoreApplication_UnhandledErrorDetected;

        }

        /// <summary>
        /// Any  uncaught exceptions are thrown to here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoreApplication_UnhandledErrorDetected(object sender, UnhandledErrorDetectedEventArgs e)
        {
            try
            {
                logChannel.LogMessage("Caught the exception");
                e.UnhandledError.Propagate();

            }
            catch (Exception ex)
            {
                //logChannel.LogMessage(string.Format("UnhandledErro: 0x{0:X})", ex.HResult), LoggingLevel.Critical);
                logChannel.LogMessage(string.Format("Effor Message: {0}", ex.Message));

                if (logSession != null)
                {
                    //var filename = DateTime.Now.ToString("yyyyMMdd-HHmmssTzz") + ".etl";
                    var filename = DateTime.Now.ToString("yyyyMMdd") + ".etl";
                    var logSaveTast = logSession
                        .SaveToFileAsync(logUploadFolder, filename)
                        .AsTask();

                    logSaveTast.Wait();
                }


                // throw;
            }
        }

        /// <summary>
        /// Deelete the files based on the days mentioned
        /// </summary>

        public async void Deletefile()
        {
            try
            {

                var logFiles = await logUploadFolder.GetFilesAsync();

                foreach (var logFile in logFiles)
                {
                    if ((DateTime.Now - logFile.DateCreated).Days > DAYS_TO_DELETE)
                    {
                        await logFile.DeleteAsync();
                    }


                }
            }
            catch (Exception ex)
            {
                logChannel.LogMessage(ex.Message);

            }
        }

    }
}
