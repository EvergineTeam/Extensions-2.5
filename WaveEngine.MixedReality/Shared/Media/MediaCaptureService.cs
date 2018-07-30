// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using WaveEngine.Common;
#if UWP
using Windows.Graphics.Imaging;
#endif
#endregion

namespace WaveEngine.MixedReality.Media
{
    /// <summary>
    /// Media capture service.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.MixedReality.Media")]
    public class MediaCaptureService : Service
    {
        /// <summary>
        /// The is connected
        /// </summary>
        private bool isConnected;

        /// <summary>
        /// Instance of videocapturemanager.
        /// </summary>
        private IVideoCapture videoCaptureManager;

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return this.videoCaptureManager != null || this.IsConnected == false;
            }
        }

        /// <summary>
        /// Gets the supported resolutions
        /// </summary>
        public List<ICameraResolution> SupportedResolutions
        {
            get
            {
                return this.videoCaptureManager?.SupportedResolutions?.ToList();
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaCaptureService"/> class.
        /// </summary>
        public MediaCaptureService()
        {
#if UWP
            this.videoCaptureManager = new VideoCaptureManager();
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize all resources used by this instance.
        /// </summary>
        protected override async void Initialize()
        {
            if (this.videoCaptureManager != null)
            {
                this.isConnected = await this.videoCaptureManager.InitializeCameraAsync();
            }
        }

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        protected override void Terminate()
        {
            if (this.videoCaptureManager != null)
            {
                (this.videoCaptureManager as IDisposable).Dispose();
            }
        }

        /// <summary>
        /// Take a new photo and hold it in the output stream.
        /// </summary>
        /// <param name="parameters">The set of parameters used to take the photo, <see cref="ICameraResolution"/> class.</param>
        /// <param name="format">The image format. <see cref="PhotoCaptureFormat"/> enum. </param>
        /// <returns>The <see cref="PhotoCaptureStreamResult"/> struct.</returns>
        public async Task<PhotoCaptureStreamResult> TakePhotoInStreamAsync(ICameraResolution parameters = null, PhotoCaptureFormat format = PhotoCaptureFormat.JPG)
        {
            PhotoCaptureStreamResult result = default(PhotoCaptureStreamResult);

            if (this.videoCaptureManager != null)
            {
                result = await this.videoCaptureManager.TakePhotoToStreamAsync(parameters, format);
            }

            return result;
        }

        /// <summary>
        /// Take a new photo.
        /// </summary>
        /// <param name="filename">The output file name.</param>
        /// <param name="parameters">The set of parameters used to take the photo, <see cref="ICameraResolution"/> class.</param>
        /// <param name="format">The image format. <see cref="PhotoCaptureFormat"/> enum. </param>
        /// <returns>The <see cref="VideoCaptureResult"/> struct.</returns>
        public async Task<PhotoCaptureResult> TakePhotoAsync(string filename, ICameraResolution parameters = null, PhotoCaptureFormat format = PhotoCaptureFormat.JPG)
        {
            PhotoCaptureResult result = default(PhotoCaptureResult);

            if (this.videoCaptureManager != null)
            {
                result = await this.videoCaptureManager.TakePhotoAsync(filename, parameters, format);
            }

            return result;
        }

        /// <summary>
        /// Start recording a new video.
        /// </summary>
        /// <param name="filename">The file path.</param>
        /// <param name="encoding">The video encoding. Mp4 by default.</param>
        /// <returns>The <see cref="VideoCaptureResult"/> struct.</returns>
        public async Task<VideoCaptureResult> StartRecordingAsync(string filename, VideoEncoding encoding = VideoEncoding.MP4)
        {
            VideoCaptureResult result = default(VideoCaptureResult);

            if (this.videoCaptureManager != null)
            {
                result = await this.videoCaptureManager.StartRecordingAsync(filename, encoding);
            }

            return result;
        }

        /// <summary>
        /// Start recording a new video and save it as stream.
        /// </summary>
        /// <param name="encoding">The video encoding. Mp4 by default.</param>
        /// <returns>The <see cref="VideoCaptureStreamResult"/> struct.</returns>
        public async Task<VideoCaptureStreamResult> StartRecordingInStreamAsync(VideoEncoding encoding = VideoEncoding.MP4)
        {
            VideoCaptureStreamResult result = default(VideoCaptureStreamResult);

            if (this.videoCaptureManager != null)
            {
                result = await this.videoCaptureManager.StartRecordingToStreamAsync(encoding);
            }

            return result;
        }

        /// <summary>
        /// Stop recording the current video.
        /// </summary>
        /// <returns>Whether the operation was executed or not.</returns>
        public async Task<bool> StopRecordingAsync()
        {
            if (this.videoCaptureManager != null)
            {
                return await this.videoCaptureManager.StopRecordingAsync();
            }

            return false;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Called when the service is resumed because the app is deactivated
        /// </summary>
        public override void OnDeactivated()
        {
            base.OnDeactivated();
        }

        /// <summary>
        /// Called when the service is resumed because the app is activated
        /// </summary>
        public override void OnActivated()
        {
            base.OnDeactivated();
            this.videoCaptureManager.OnActivated();
        }

        #endregion
    }
}
