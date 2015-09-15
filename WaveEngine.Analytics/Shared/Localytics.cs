#region File Description
//-----------------------------------------------------------------------------
// Localytics
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using WaveEngine.Common;
#endregion

namespace WaveEngine.Analytics
{
    /// <summary>
    /// This class represent a Localytics system.
    /// </summary>
    public class Localytics : AnalyticsSystem
    {
        /// <summary>
        /// Max stored sessions allow.
        /// </summary>
        private const int MaxStoredSessions = 10;

        /// <summary>
        /// Library version.
        /// </summary>
        private const string LibraryVersion = "windowsphone_2.2";

        /// <summary>
        /// Directory name using in Isolated Storage.
        /// </summary>
        private const string DirectoryName = "localytics";

        /// <summary>
        /// Session file prefix.
        /// </summary>
        private const string SessionFilePrefix = "s_";

        /// <summary>
        /// Upload file prefix.
        /// </summary>
        private const string UploadFilePrefix = "u_";

        /// <summary>
        /// Meta file name.
        /// </summary>
        private const string MetaFileName = "m_meta";

        /// <summary>
        /// Servuce base url.
        /// </summary>
        private const string ServiceUrlBase = "http://analytics.localytics.com/api/v2/applications/";

        /// <summary>
        /// Represent a app key.
        /// </summary>
        private string appKey;

        /// <summary>
        /// Represent a session id
        /// </summary>
        private string sessionUuid;

        /// <summary>
        /// Session file name.
        /// </summary>
        private string sessionFilename;

        /// <summary>
        /// Whether session is open.
        /// </summary>
        private bool isSessionOpen = false;

        /// <summary>
        /// Whether session is closed.
        /// </summary>
        private bool isSessionClosed = false;

        /// <summary>
        /// Session start time.
        /// </summary>
        private double sessionStartTime = 0;

        /// <summary>
        /// Whether the current status is upload.
        /// </summary>
        private bool isUploading = false;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="Localytics"/> class.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <param name="analyticsInfo">The analytics info.</param>
        public Localytics(IAdapter adapter, AnalyticsInfo analyticsInfo)
            : base(adapter, analyticsInfo)
        {
            LocalyticsInfo info = analyticsInfo as LocalyticsInfo;

            this.appKey = info.AppKey;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Opens or resumes the Localytics session.
        /// </summary>
        public override void Open()
        {
            if (this.isSessionOpen || this.isSessionClosed)
            {
                this.LogMessage("Session is already opened or closed.");
                return;
            }

            try
            {
                if (this.GetNumberOfStoredSessions() > MaxStoredSessions)
                {
                    this.LogMessage("Local stored session count exceeded.");
                    return;
                }

                this.sessionUuid = Guid.NewGuid().ToString();
                this.sessionFilename = SessionFilePrefix + this.sessionUuid;
                this.sessionStartTime = this.GetTimeInUnixTime();

                //// Format of an open session:
                ////{ "dt":"s",       // This is a session blob
                ////  "ct": long,     // seconds since Unix epoch
                ////  "u": string     // A unique ID attached to this session 
                ////  "nth": int,     // This is the nth session on the device. (not required)
                ////  "new": boolean, // New vs returning (not required)
                ////  "sl": long,     // seconds since last session (not required)
                ////  "lat": double,  // latitude (not required)
                ////  "lng": double,  // longitude (not required)
                ////  "c0" : string,  // custom dimensions (not required)
                ////  "c1" : string,
                ////  "c2" : string,
                ////  "c3" : string }

                StringBuilder openstring = new StringBuilder();
                openstring.Append("{\"dt\":\"s\",");
                openstring.Append("\"ct\":" + this.GetTimeInUnixTime().ToString() + ",");
                openstring.Append("\"u\":\"" + this.sessionUuid + "\"");
                openstring.Append("}");
                openstring.Append(Environment.NewLine);

                this.AppendTextToFile(openstring.ToString(), this.sessionFilename);

                this.isSessionOpen = true;
                this.LogMessage("Session opened.");
            }
            catch (Exception e)
            {
                this.LogMessage("Swallowing exception: " + e.Message);
            }
        }

        /// <summary>
        /// Closes the Localytics session.
        /// </summary>
        public override void Close()
        {
            if (this.isSessionOpen == false || this.isSessionClosed == true)
            {
                this.LogMessage("Session not closed b/c it is either not open or already closed.");
                return;
            }

            try
            {
                ////{ "dt":"c", // close data type
                ////  "u":"abec86047d-ae51", // unique id for teh close
                ////  "ss": session_start_time, // time the session was started
                ////  "su":"696c44ebf6f",   // session uuid
                ////  "ct":1302559195,  // client time
                ////  "ctl":114,  // session length (optional)
                ////  "cta":60, // active time length (optional)
                ////  "fl":["1","2","3","4","5","6","7","8","9"], // Flows (optional)
                ////  "lat": double,  // lat (optional)
                ////  "lng": double,  // lng (optional)
                ////  "c0" : string,  // custom dimensions (otpinal)
                ////  "c1" : string,
                ////  "c2" : string,
                ////  "c3" : string }

                StringBuilder closeString = new StringBuilder();
                closeString.Append("{\"dt\":\"c\",");
                closeString.Append("\"u\":\"" + Guid.NewGuid().ToString() + "\",");
                closeString.Append("\"ss\":" + this.sessionStartTime.ToString() + ",");
                closeString.Append("\"su\":\"" + this.sessionUuid + "\",");
                closeString.Append("\"ct\":" + this.GetTimeInUnixTime().ToString());
                closeString.Append("}");
                closeString.Append(Environment.NewLine);
                this.AppendTextToFile(closeString.ToString(), this.sessionFilename); // the close blob

                this.isSessionOpen = false;
                this.isSessionClosed = true;
                this.LogMessage("Session closed.");
            }
            catch (Exception e)
            {
                this.LogMessage("Swallowing exception: " + e.Message);
            }
        }

        /// <summary>
        /// Creates a new thread which collects any files and uploads them. Returns immediately if an upload
        /// is already happenning.
        /// </summary>
        public override void Upload()
        {
            if (this.isUploading)
            {
                return;
            }

            this.isUploading = true;
            this.BeginUpload();
        }

        /// <summary>
        /// Records a specific event as having occured and optionally records some attributes associated with this event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        public override void TagEvent(string eventName, string attribute, string value)
        {
            var attributes = new Dictionary<string, string>();
            attributes.Add(attribute, value);

            this.TagEvent(eventName, attributes);
        }

        /// <summary>
        /// Records a specific event as having occured and optionally records some attributes associated with this event.
        /// This should not be called inside a loop. It should not be used to record personally identifiable information
        /// and it is best to define all your event names rather than generate them programatically.
        /// </summary>
        /// <param name="eventName">The name of the event which occured. E.G. 'button pressed'</param>
        /// <param name="attributes">Key value pairs that record data relevant to the event.</param>
        public override void TagEvent(string eventName, Dictionary<string, string> attributes)
        {
            if (this.isSessionOpen == false)
            {
                this.LogMessage("Event not tagged because session is not open.");
                return;
            }

            ////{ "dt":"e",  // event data time
            ////  "ct":1302559181,   // client time
            ////  "u":"48afd8beebd3",   // unique id
            ////  "su":"696c44ebf6f",   // session id
            ////  "n":"Button Clicked",  // event name
            ////  "lat": double,   // lat (optional)
            ////  "lng": double,   // lng (optional)
            ////  "attrs":   // event attributes (optional)
            ////  {
            ////      "Button Type":"Round"
            ////  },
            ////  "c0" : string, // custom dimensions (optional)
            ////  "c1" : string,
            ////  "c2" : string,
            ////  "c3" : string }

            try
            {
                StringBuilder eventString = new StringBuilder();
                eventString.Append("{\"dt\":\"e\",");
                eventString.Append("\"ct\":" + this.GetTimeInUnixTime().ToString() + ",");
                eventString.Append("\"u\":\"" + Guid.NewGuid().ToString() + "\",");
                eventString.Append("\"su\":\"" + this.sessionUuid + "\",");
                eventString.Append("\"n\":" + this.EscapeString(eventName));

                if (attributes != null)
                {
                    eventString.Append(",\"attrs\": {");
                    bool first = true;
                    foreach (string key in attributes.Keys)
                    {
                        if (!first)
                        {
                            eventString.Append(",");
                        }

                        eventString.Append(this.EscapeString(key) + ":" + this.EscapeString(attributes[key]));
                        first = false;
                    }

                    eventString.Append("}");
                }

                eventString.Append("}");
                eventString.Append(Environment.NewLine);

                this.AppendTextToFile(eventString.ToString(), this.sessionFilename); // the close blob
                this.LogMessage("Tagged event: " + this.EscapeString(eventName));
            }
            catch (Exception e)
            {
                this.LogMessage("Swallowing exception: " + e.Message);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the current date/time as a string which can be inserted in the DB
        /// </summary>
        /// <returns>A formatted string with date, time, and timezone information</returns>
        private string GetDatestring()
        {
            DateTime dt = DateTime.Now.ToUniversalTime();

            // reformat the time to: YYYY-MM-DDTHH:MM:SS
            // use a StringBuilder to avoid creating multiple 
            StringBuilder datestring = new StringBuilder();
            datestring.Append(dt.Year);
            datestring.Append("-");
            datestring.Append(dt.Month.ToString("D2"));
            datestring.Append("-");
            datestring.Append(dt.Day.ToString("D2"));
            datestring.Append("T");
            datestring.Append(dt.Hour.ToString("D2"));
            datestring.Append(":");
            datestring.Append(dt.Minute.ToString("D2"));
            datestring.Append(":");
            datestring.Append(dt.Second.ToString("D2"));

            return datestring.ToString();
        }

        /// <summary>
        /// Gets the current time in unixtime
        /// </summary>
        /// <returns>The current time in unixtime</returns>
        private double GetTimeInUnixTime()
        {
            return Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds, 0);
        }

        /// <summary>
        /// Constructs a blob header for uploading
        /// </summary>
        /// <returns>A string containing a blob header</returns>
        private string GetBlobHeader()
        {
            StringBuilder blobString = new StringBuilder();

            ////{ "dt":"h",  // data type, h for header
            ////  "pa": int, // persistent store created at
            ////  "seq": int,  // blob sequence number, incremented on each new blob, 
            ////               // remembered in the persistent store
            ////  "u": string, // A unique ID for the blob. Must be the same if the blob is re-uploaded!
            ////  "attrs": {
            ////    "dt": "a" // data type, a for attributes
            ////    "au":string // Localytics Application Id
            ////    "du":string // Device UUID
            ////    "s":boolean // Whether the app has been stolen (optional)
            ////    "j":boolean // Whether the device has been jailbroken (optional)
            ////    "lv":string // Library version
            ////    "av":string // Application version
            ////    "dp":string // Device Platform
            ////    "dll":string // Locale Language (optional)
            ////    "dlc":string // Locale Country (optional)
            ////    "nc":string // Network Country (iso code) (optional)
            ////    "dc":string // Device Country (iso code) (optional)
            ////    "dma":string // Device Manufacturer (optional)
            ////    "dmo":string // Device Model
            ////    "dov":string // Device OS Version
            ////    "nca":string // Network Carrier (optional)
            ////    "dac":string // Data Connection Type (optional)
            ////    "mnc":int // mobile network code (optional)
            ////    "mcc":int // mobile country code (optional)
            ////    "tdid":string // Telephony Device Id (meid or imei) (optional)
            ////    "wmac":string // hashed wifi mac address (optional)
            ////    "emac":string // hashed ethernet mac address (optional)
            ////    "bmac":string // hashed bluetooth mac address (optional)
            ////    "iu":string // install id
            ////    "udid":string } } // client side hashed version of the udid
            blobString.Append("{\"dt\":\"h\",");
            blobString.Append("\"pa\":" + this.GetPersistStoreCreateTime() + ",");

            string sequenceNumber = this.GetSequenceNumber();
            blobString.Append("\"seq\":" + sequenceNumber + ",");
            this.SetNextSequenceNumber((int.Parse(sequenceNumber) + 1).ToString());

            blobString.Append("\"u\":\"" + Guid.NewGuid().ToString() + "\",");
            blobString.Append("\"attrs\":");
            blobString.Append("{\"dt\":\"a\",");
            blobString.Append("\"au\":\"" + this.appKey + "\",");
            blobString.Append("\"du\":\"" + Adapter.DeviceUniqueID + "\",");
            blobString.Append("\"lv\":\"" + LibraryVersion + "\",");
            blobString.Append("\"av\":\"" + Adapter.AppVersion + "\",");
            blobString.Append("\"dp\":\"" + Adapter.Platform + "\",");
            blobString.Append("\"dll\":\"" + Adapter.LocaleLanguage + "\",");
            blobString.Append("\"dma\":\"" + Adapter.DeviceMake + "\",");
            blobString.Append("\"dmo\":\"" + Adapter.DeviceModel + "\",");
            blobString.Append("\"dov\":\"" + Adapter.OSVersion + "\",");
            blobString.Append("\"iu\":\"" + this.GetInstallId() + "\"");

            blobString.Append("}}");
            blobString.Append(Environment.NewLine);

            return blobString.ToString();
        }

        /// <summary>
        /// Gets the Installation ID out of the metadata file
        /// </summary>
        /// <returns>String result.</returns>
        private string GetInstallId()
        {
            string installID;
            var store = Adapter.IOManager;
            using (var file = store.OpenStorageFile(Path.Combine(DirectoryName, MetaFileName), WaveEngine.Common.IO.FileMode.Open))
            using (TextReader reader = new StreamReader(file))
            {
                installID = reader.ReadLine();
            }

            return installID;
        }

        /// <summary>
        /// Gets the sequence number for the next upload blob. 
        /// </summary>
        /// <returns>Sequence number as a string</returns>
        private string GetSequenceNumber()
        {
            // open the meta file and read the next sequence number.
            var store = Adapter.IOManager;
            string metaFile = Path.Combine(DirectoryName, MetaFileName);
            if (!store.ExistsStorageFile(metaFile))
            {
                this.SetNextSequenceNumber("1");
                return "1";
            }

            string sequenceNumber;
            using (var file = store.OpenStorageFile(Path.Combine(DirectoryName, MetaFileName), WaveEngine.Common.IO.FileMode.Open))
            using (TextReader reader = new StreamReader(file))
            {
                reader.ReadLine();
                sequenceNumber = reader.ReadLine();
            }

            return sequenceNumber;
        }

        /// <summary>
        /// Sets the next sequence number in the metadata file. Creates the file if its not already there
        /// </summary>
        /// <param name="number">Next sequence number</param>
        private void SetNextSequenceNumber(string number)
        {
            var store = Adapter.IOManager;
            string metaFile = Path.Combine(DirectoryName, MetaFileName);
            if (!store.ExistsStorageFile(metaFile))
            {
                // Create a new metadata file consisting of a unique installation ID and a sequence number
                this.AppendTextToFile(Guid.NewGuid().ToString() + Environment.NewLine + number, MetaFileName);
            }
            else
            {
                string installId;
                using (var fileIn = store.OpenStorageFile(metaFile, WaveEngine.Common.IO.FileMode.Open))
                using (TextReader reader = new StreamReader(fileIn))
                {
                    installId = reader.ReadLine();
                }

                // overwite the file w/ the old install ID and the new sequence number
                using (var fileOut = store.OpenStorageFile(metaFile, WaveEngine.Common.IO.FileMode.Truncate))
                using (TextWriter writer = new StreamWriter(fileOut))
                {
                    writer.WriteLine(installId);
                    writer.Write(number);
                }
            }
        }

        /// <summary>
        /// Gets the timestamp of the storage file containing the sequence numbers. This allows processing to
        /// ignore duplicates or identify missing uploads
        /// </summary>
        /// <returns>A string containing a Unixtime</returns>
        private string GetPersistStoreCreateTime()
        {
            var store = Adapter.IOManager;
            string metaFile = Path.Combine(DirectoryName, MetaFileName);
            if (!store.ExistsStorageFile(metaFile))
            {
                this.SetNextSequenceNumber("1");
            }

            DateTimeOffset dto = store.GetCreationTime(metaFile);
            int secondsSinceUnixEpoch = (int)Math.Round((dto.DateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds);
            return secondsSinceUnixEpoch.ToString();
        }

        /// <summary>
        /// Formats an input string for YAML
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// string sorrounded in quotes, with dangerous characters escaped
        /// </returns>
        private string EscapeString(string input)
        {
            string escapedSlahes = input.Replace("\\", "\\\\");
            return "\"" + escapedSlahes.Replace("\"", "\\\"") + "\"";
        }

        /// <summary>
        /// Outputs a message to the debug console
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private void LogMessage(string msg)
        {
            Debug.WriteLine("(localytics) " + msg);
        }

        #region Storage

        /// <summary>
        /// Tallies up the number of files whose name starts w/ sessionFilePrefix in the localytics dir
        /// </summary>
        /// <returns>Number of stored sessions.</returns>
        private int GetNumberOfStoredSessions()
        {
            var store = Adapter.IOManager;
            if (store.DirectoryExists(DirectoryName) == false)
            {
                return 0;
            }

            return store.GetFileNames(DirectoryName + @"/" + SessionFilePrefix + "*").Length;
        }

        /// <summary>
        /// Appends a string to the end of a text file
        /// </summary>
        /// <param name="text">Text to append</param>
        /// <param name="filename">Name of file to append to</param>
        private void AppendTextToFile(string text, string filename)
        {
            // GetStreamForFile integrated inside this method
            var store = Adapter.IOManager;
            if (!store.DirectoryExists(DirectoryName))
            {
                store.CreateDirectory(DirectoryName);
            }

            string name = Path.Combine(DirectoryName, filename);
            using (Stream file = store.OpenStorageFile(name, WaveEngine.Common.IO.FileMode.Append))                      
            using (TextWriter writer = new StreamWriter(file))
            {                
                writer.Write(text);
            }
        }

        /// <summary>
        /// Reads a file and returns its contents as a string
        /// </summary>
        /// <param name="filename">file to read (w/o directory prefix)</param>
        /// <returns>the contents of the file</returns>
        private string GetFileContents(string filename)
        {
            var store = Adapter.IOManager;
            string contents;
            using (var file = store.OpenStorageFile(Path.Combine(DirectoryName, filename), WaveEngine.Common.IO.FileMode.Open))
            using (TextReader reader = new StreamReader(file))
            {
                contents = reader.ReadToEnd();
            }

            return contents;
        }
        #endregion

        #region upload

        /// <summary>
        /// Goes through all the upload files and collects their contents for upload
        /// </summary>
        /// <returns>A string containing the concatenated </returns>
        private string GetUploadContents()
        {
            StringBuilder contents = new StringBuilder();
            var store = Adapter.IOManager;

            if (store.DirectoryExists(DirectoryName))
            {
                string[] files = store.GetFileNames(DirectoryName + @"/" + UploadFilePrefix + "*");
                foreach (string file in files)
                {
                    // workaround for GetFileNames bug
                    if (file.StartsWith(UploadFilePrefix))
                    {
                        contents.Append(this.GetFileContents(file));
                    }
                }
            }

            return contents.ToString();
        }

        /// <summary>
        /// loops through all the files in the directory deleting the upload files
        /// </summary>
        private void DeleteUploadFiles()
        {
            var store = Adapter.IOManager;

            if (store.DirectoryExists(DirectoryName))
            {
                string[] files = store.GetFileNames(DirectoryName + @"/" + UploadFilePrefix + "*");
                foreach (string file in files)
                {
                    // workaround for GetfileNames returning extra files
                    if (file.StartsWith(UploadFilePrefix))
                    {
                        store.DeleteStorageFile(Path.Combine(DirectoryName, file));
                    }
                }
            }
        }

        /// <summary>
        /// Rename any open session files. This way events recorded during uploaded get written safely to disk
        /// and threading difficulties are missed.
        /// </summary>
        private void RenameOrAppendSessionFiles()
        {
            var store = Adapter.IOManager;

            if (store.DirectoryExists(DirectoryName))
            {                
                string[] files = store.GetFileNames(DirectoryName + @"/" + SessionFilePrefix + "*");
                string destinationFilename = UploadFilePrefix + Guid.NewGuid().ToString();

                bool addedHeader = false;
                foreach (string file in files)
                {
                    // work around for GetFileNames returning extra files
                    if (file.StartsWith(SessionFilePrefix))
                    {
                        // Any time sessions are appended, an upload header should be added. But only one is needed regardless of number of files added
                        if (!addedHeader)
                        {
                            this.AppendTextToFile(this.GetBlobHeader(), destinationFilename);
                            addedHeader = true;
                        }

                        this.AppendTextToFile(this.GetFileContents(file), destinationFilename);
                        store.DeleteStorageFile(Path.Combine(DirectoryName, file));
                    }
                }
            }
        }

        /// <summary>
        /// Runs on a seperate thread and is responsible for renaming and uploading files as appropriate
        /// </summary>
        private void BeginUpload()
        {
            this.LogMessage("Beginning upload.");

            try
            {
                this.RenameOrAppendSessionFiles();

                // begin the upload
                string url = ServiceUrlBase + this.appKey + "/uploads";
                this.LogMessage("Uploading to: " + url);
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "POST";
                myRequest.ContentType = "application/json";
                myRequest.BeginGetRequestStream(new AsyncCallback(this.HttpRequestCallback), myRequest);
            }
            catch (Exception e)
            {
                this.LogMessage("Swallowing exception: " + e.Message);
            }
        }

        /// <summary>
        /// HTTPs the request callback.
        /// </summary>
        /// <param name="asynchronousResult">The asynchronous result.</param>
        private void HttpRequestCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                using (Stream postStream = request.EndGetRequestStream(asynchronousResult))
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(this.GetUploadContents());
                    postStream.Write(byteArray, 0, byteArray.Length);
                }

                request.BeginGetResponse(new AsyncCallback(this.GetResponseCallback), request);
            }
            catch (Exception e)
            {
                this.LogMessage("Swallowing exception: " + e.Message);
            }
        }

        /// <summary>
        /// Gets the response callback.
        /// </summary>
        /// <param name="asynchronousResult">The asynchronous result.</param>
        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult))
                using (Stream streamResponse = response.GetResponseStream())
                using (StreamReader streamRead = new StreamReader(streamResponse))
                {
                    string responseString = streamRead.ReadToEnd();

                    this.LogMessage("Upload complete. Response: " + responseString);
                    this.DeleteUploadFiles();
                }
            }
            catch (WebException e)
            {
                Debug.WriteLine("WebException raised.");
                Debug.WriteLine("\n{0}", e.Message);
                Debug.WriteLine("\n{0}", e.Status);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception raised!");
                Debug.WriteLine("Message : " + e.Message);
            }
            finally
            {
                this.isUploading = false;
            }
        }
        #endregion

        #endregion
    }
}
