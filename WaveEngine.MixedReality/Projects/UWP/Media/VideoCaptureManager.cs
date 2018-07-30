// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Graphics.Imaging;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.MixedReality.Media
{
    /// <summary>
    /// This class the concrete implementation of IVideoCapture on Universal Windows Platforms.
    /// </summary>
    internal class VideoCaptureManager : IVideoCapture, IDisposable
    {
        private MediaCapture mediaCapture;
        private bool isInitialized;
        private bool isRecording;
        private bool isPreviewing;
        private IEnumerable<StreamResolution> allStreamResolutions;
        private StorageFolder captureFolder;
        private bool disposed;

        #region Properties

        /// <summary>
        /// Gets the supported resolutions.
        /// </summary>
        public IEnumerable<ICameraResolution> SupportedResolutions
        {
            get
            {
                if (!this.isInitialized)
                {
                    throw new InvalidOperationException("VideoCapture is not initialized.");
                }

                return this.allStreamResolutions;
            }
        }

        /// <summary>
        /// Gets a value indicating whether whether the video capture is recording or not.
        /// </summary>
        public bool IsRecording
        {
            get
            {
                return this.isRecording;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureManager"/> class.
        /// </summary>
        public VideoCaptureManager()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="VideoCaptureManager" /> class.
        /// </summary>
        ~VideoCaptureManager()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resumes the manager
        /// </summary>
        public async void OnActivated()
        {
            if (this.isInitialized)
            {
                this.isInitialized = false;
                this.mediaCapture = null;

                await this.InitializeCameraAsync();
            }
        }

        /// <summary>
        /// Initializes this camera instance asynchronous.
        /// </summary>
        /// <returns>Whether the camera was initialized or not.</returns>
        public async Task<bool> InitializeCameraAsync()
        {
            Debug.WriteLine("InitializeCameraAync");

            if (this.mediaCapture == null)
            {
                // Get available devices for capturing pictures
                var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                if (allVideoDevices.Count == 0)
                {
                    Debug.WriteLine("There is not any camera device detected.");
                    return false;
                }

                // Get the desired camera by panel
                DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == Panel.Back);

                // If there is no device mounted on the desired panel, return the first device found
                var cameraDevice = desiredDevice ?? allVideoDevices.FirstOrDefault();

                this.mediaCapture = new MediaCapture();

                // Register for a notification when video recording has reached the maximum time and when something goes wrong
                this.mediaCapture.RecordLimitationExceeded += this.MediaCapture_RecordLimitationExceeded;
                this.mediaCapture.Failed += this.MediaCapture_Failed;

                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                try
                {
                    await this.mediaCapture.InitializeAsync(settings);
                    this.isInitialized = true;
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("the app was denied access to the camera.");
                }

                if (this.isInitialized)
                {
                    this.allStreamResolutions = this.mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).Select(x => new StreamResolution(x));

                    var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);

                    // Fall back to the local app storage if the Pictures Library is not available
                    this.captureFolder = picturesLibrary.SaveFolder ?? ApplicationData.Current.LocalFolder;
                }
            }

            return this.isInitialized;
        }

        /// <summary>
        /// Take a new photo and hold it in the output stream.
        /// </summary>
        /// <param name="parameters">The set of parameters used to take the photo, <see cref="ICameraResolution"/> class.</param>
        /// <param name="format">The image format. <see cref="PhotoCaptureFormat"/> enum. </param>
        /// <returns>The <see cref="PhotoCaptureStreamResult"/> struct.</returns>
        public async Task<PhotoCaptureStreamResult> TakePhotoToStreamAsync(ICameraResolution parameters, PhotoCaptureFormat format)
        {
            if (!this.isInitialized)
            {
                Debug.WriteLine("First you need to initialize the videocapturemanager.");
                return new PhotoCaptureStreamResult();
            }

            try
            {
                if (parameters != null)
                {
                    Debug.WriteLine("Applying paramenters...");
                    await this.mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, (parameters as StreamResolution).EncodingProperties);
                }

                Debug.WriteLine("Taking photo...");
                var stream = new InMemoryRandomAccessStream();

                var properties = this.GetImageEncodingProperties(format);
                await this.mediaCapture.CapturePhotoToStreamAsync(properties, stream);

                Debug.WriteLine("Photo stream loaded.");
                return new PhotoCaptureStreamResult(true, stream.AsStream());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when taking a photo: " + ex.ToString());
                return new PhotoCaptureStreamResult(false, Stream.Null);
            }
        }

        /// <summary>
        /// Take a new photo.
        /// </summary>
        /// <param name="filename">The output file name.</param>
        /// <param name="parameters">The set of parameters used to take the photo, <see cref="ICameraResolution"/> class.</param>
        /// <param name="format">The image format. <see cref="PhotoCaptureFormat"/> enum. </param>
        /// <returns>The <see cref="VideoCaptureResult"/> struct.</returns>
        public async Task<PhotoCaptureResult> TakePhotoAsync(string filename, ICameraResolution parameters, PhotoCaptureFormat format)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("The filename cannot be empty or null.");
            }

            if (!this.isInitialized)
            {
                Debug.WriteLine("First you need to initialize the videocapturemanager.");
                return new PhotoCaptureResult();
            }

            try
            {
                if (parameters != null)
                {
                    Debug.WriteLine("Applying paramenters...");
                    await this.mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, (parameters as StreamResolution).EncodingProperties);
                }

                Debug.WriteLine("Taking photo...");
                var properties = this.GetImageEncodingProperties(format);
                var name = filename + this.GetFormatExtension(format);
                var file = await this.captureFolder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);

                Debug.WriteLine("Photo taken! Saving to " + file.Path);
                await this.mediaCapture.CapturePhotoToStorageFileAsync(properties, file);

                Debug.WriteLine("Photo saved.");
                return new PhotoCaptureResult(true, file.Path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when taking a photo: " + ex.ToString());
                return new PhotoCaptureResult(false, string.Empty);
            }
        }

        /// <summary>
        /// Start the preview.
        /// </summary>
        /// <returns>Whether the operation was executed or not.</returns>
        public async Task<bool> StartPreviewAsync()
        {
            Debug.WriteLine("StartPreviewAsync");

            if (!this.isInitialized)
            {
                Debug.WriteLine("First you need to initialize the videocapturemanager.");
                return false;
            }
            else
            {
                await this.mediaCapture.StartPreviewAsync();
                this.isPreviewing = true;
                return true;
            }
        }

        /// <summary>
        /// Stop the preview.
        /// </summary>
        /// <returns>Whether the operation was executed or not.</returns>
        public async Task<bool> StopPreviewAsync()
        {
            Debug.WriteLine("StopPreviewAsync");

            if (!this.isInitialized)
            {
                Debug.WriteLine("First you need to initialize the videocapturemanager.");
                return false;
            }
            else
            {
                await this.mediaCapture.StopPreviewAsync();
                this.isPreviewing = false;
                return true;
            }
        }

        /// <summary>
        /// Gets the current preview frame.
        /// </summary>
        /// <returns>The <see cref="SoftwareBitmap"/> result.</returns>
        public async Task<SoftwareBitmap> GetPreviewFrameAsync()
        {
            if (!this.isInitialized)
            {
                Debug.WriteLine("First you need to initialize the videocapturemanager.");
                return null;
            }

            if (!this.isPreviewing)
            {
                Debug.WriteLine("First you need to start the preview.");
                return null;
            }

            try
            {
                var frame = await this.mediaCapture.GetPreviewFrameAsync();
                return frame.SoftwareBitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Start recording a new video.
        /// </summary>
        /// <param name="filename">The file path.</param>
        /// <param name="encoding">The video encoding. Mp4 by default.</param>
        /// <returns>The <see cref="VideoCaptureResult"/> struct.</returns>
        public async Task<VideoCaptureResult> StartRecordingAsync(string filename, VideoEncoding encoding)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("The filename cannot be empty or null.");
            }

            if (!this.isInitialized)
            {
                Debug.WriteLine("First you need to initialize the videocapturemanager.");
                return new VideoCaptureResult();
            }

            if (this.IsRecording)
            {
                Debug.WriteLine("VideoCapture is already recording. Call Stop before Start again.");
                return new VideoCaptureResult(false, string.Empty);
            }

            try
            {
                Debug.WriteLine("Recording video...");
                var name = filename + ".mp4";
                var file = await this.captureFolder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
                MediaEncodingProfile encodingProfile;

                switch (encoding)
                {
                    case VideoEncoding.AVI:
                        encodingProfile = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.Auto);
                        break;
                    case VideoEncoding.WMV:
                        encodingProfile = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Auto);
                        break;
                    default:
                    case VideoEncoding.MP4:
                        encodingProfile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
                        break;
                }

                await this.mediaCapture.StartRecordToStorageFileAsync(encodingProfile, file);
                this.isRecording = true;

                Debug.WriteLine("Video saved.");
                return new VideoCaptureResult(true, file.Path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when taking a photo: " + ex.ToString());
                return new VideoCaptureResult(false, string.Empty);
            }
        }

        /// <summary>
        /// Start recording a new video and save it as stream.
        /// </summary>
        /// <param name="encoding">The video encoding. Mp4 by default.</param>
        /// <returns>The <see cref="VideoCaptureStreamResult"/> struct.</returns>
        public async Task<VideoCaptureStreamResult> StartRecordingToStreamAsync(VideoEncoding encoding)
        {
            if (!this.isInitialized)
            {
                Debug.WriteLine("First you need to initialize the videocapturemanager.");
                return new VideoCaptureStreamResult();
            }

            if (this.IsRecording)
            {
                Debug.WriteLine("VideoCapture is already recording. Call Stop before Start again.");
                return new VideoCaptureStreamResult(false, Stream.Null);
            }

            try
            {
                Debug.WriteLine("Recording video...");
                MediaEncodingProfile encodingProfile;

                switch (encoding)
                {
                    case VideoEncoding.AVI:
                        encodingProfile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
                        break;
                    case VideoEncoding.WMV:
                        encodingProfile = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Auto);
                        break;
                    default:
                    case VideoEncoding.MP4:
                        encodingProfile = MediaEncodingProfile.CreateAvi(VideoEncodingQuality.Auto);
                        break;
                }

                var stream = new InMemoryRandomAccessStream();
                await this.mediaCapture.StartRecordToStreamAsync(encodingProfile, stream);
                this.isRecording = true;

                return new VideoCaptureStreamResult(true, stream.AsStream());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when recording a video: " + ex.ToString());
                return new VideoCaptureStreamResult(false, Stream.Null);
            }
        }

        /// <summary>
        /// Stop recording the current video.
        /// </summary>
        /// <returns>Whether the operation was executed or not.</returns>
        public async Task<bool> StopRecordingAsync()
        {
            if (!this.isInitialized)
            {
                Debug.WriteLine("First you need to initialize the videocapturemanager.");
                return false;
            }

            if (this.IsRecording)
            {
                Debug.WriteLine("Stopping recording...");

                this.isRecording = false;
                await this.mediaCapture.StopRecordAsync();
                return true;
            }
            else
            {
                Debug.WriteLine("Error: VideoCapture is not recording, you need to call methods to start recording before.");
                return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the Image encondings properties.
        /// </summary>
        /// <param name="format">The selected format.</param>
        /// <returns>The properties of the selected format.</returns>
        private ImageEncodingProperties GetImageEncodingProperties(PhotoCaptureFormat format)
        {
            switch (format)
            {
                case PhotoCaptureFormat.JPG:
                    return ImageEncodingProperties.CreateJpeg();
                case PhotoCaptureFormat.BMP:
                    return ImageEncodingProperties.CreateBmp();
                case PhotoCaptureFormat.PNG:
                default:
                    return ImageEncodingProperties.CreatePng();
            }
        }

        /// <summary>
        /// Gets the file extension string.
        /// </summary>
        /// <param name="format">The selected format.</param>
        /// <returns>This string represent a file extension.</returns>
        private string GetFormatExtension(PhotoCaptureFormat format)
        {
            switch (format)
            {
                case PhotoCaptureFormat.JPG:
                    return ".jpg";
                case PhotoCaptureFormat.BMP:
                    return ".bmp";
                case PhotoCaptureFormat.PNG:
                default:
                    return ".png";
            }
        }

        /// <summary>
        /// This is a notification that recording has to stop, and the app is expected to finalize the recording
        /// </summary>
        /// <param name="sender">The media capture instance</param>
        private async void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
        {
            Debug.WriteLine("MediaCapture_RecordLimitationExceeded");
            this.isRecording = false;
            await this.mediaCapture.StopRecordAsync();
        }

        /// <summary>
        /// Raised when an error occurs during media capture.
        /// </summary>
        /// <param name="sender">The media capture.</param>
        /// <param name="errorEventArgs">The error.</param>
        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);
            this.Dispose();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private async void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.isInitialized)
                    {
                        // If a recording is in progress during cleanup, stop it to save the recording
                        if (this.isRecording)
                        {
                            this.isRecording = false;
                            await this.mediaCapture.StopRecordAsync();
                        }

                        if (this.isPreviewing)
                        {
                            this.isPreviewing = false;
                            await this.mediaCapture.StopPreviewAsync();
                        }

                        this.isInitialized = false;

                        if (this.mediaCapture != null)
                        {
                            this.mediaCapture.RecordLimitationExceeded -= this.MediaCapture_RecordLimitationExceeded;
                            this.mediaCapture.Failed -= this.MediaCapture_Failed;
                            this.mediaCapture.Dispose();
                            this.mediaCapture = null;
                        }
                    }
                }

                this.disposed = true;
            }
        }
        #endregion
    }
}
