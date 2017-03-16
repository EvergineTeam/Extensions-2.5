#region File Description
//-----------------------------------------------------------------------------
// IVideoCapture
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using System.Threading.Tasks;
#if UWP
using Windows.Graphics.Imaging;
#endif
#endregion

namespace WaveEngine.Hololens.Media
{
    /// <summary>
    /// This interface contains all methods and properties to define a videocapture manager.
    /// </summary>
    internal interface IVideoCapture
    {
        /// <summary>
        /// Gets the supported resolutions.
        /// </summary>
        IEnumerable<ICameraResolution> SupportedResolutions { get; }

        /// <summary>
        /// Whether the videocapture is recording or not.
        /// </summary>
        bool IsRecording { get; }

        /// <summary>
        /// Initializes this camera instance asynchronous.
        /// </summary>
        /// <returns>Whether the camera was initialized or not.</returns>
        Task<bool> InitializeCameraAsync();

        /// <summary>
        /// Take a new photo and hold it as stream.
        /// </summary>
        /// <param name="parameters">The set of parameters used to take the photo, <see cref="ICameraResolution"/> class.</param>
        /// <param name="format">The image format. <see cref="PhotoCaptureFormat"/> enum. </param>
        /// <returns>The <see cref="PhotoCaptureStreamResult"/> struct.</returns>
        Task<PhotoCaptureStreamResult> TakePhotoToStreamAsync(ICameraResolution parameters, PhotoCaptureFormat format);

        /// <summary>
        /// Take a new photo.
        /// </summary>
        /// <param name="filename">The output file name.</param>
        /// <param name="parameters">The set of parameters used to take the photo, <see cref="ICameraResolution"/> class.</param>
        /// <param name="format">The image format. <see cref="PhotoCaptureFormat"/> enum. </param>
        /// <returns>The <see cref="VideoCaptureResult"/> struct.</returns>
        Task<PhotoCaptureResult> TakePhotoAsync(string filename, ICameraResolution parameters, PhotoCaptureFormat format);

        /// <summary>
        /// Start recording a new video.
        /// </summary>
        /// <param name="filename">The file path.</param>
        /// <param name="encoding">The video encoding. Mp4 by default.</param>
        /// <returns>The <see cref="VideoCaptureResult"/> struct.</returns>
        Task<VideoCaptureResult> StartRecordingAsync(string filename, VideoEncoding encoding);

        /// <summary>
        /// Start recording a new video and save it as stream.
        /// </summary>
        /// <param name="encoding">The video encoding. Mp4 by default.</param>
        /// <returns>The <see cref="VideoCaptureStreamResult"/> struct.</returns>
        Task<VideoCaptureStreamResult> StartRecordingToStreamAsync(VideoEncoding encoding);

        /// <summary>
        /// Stop recording the current video.
        /// </summary>
        /// <returns>Whether the operation was executed or not.</returns>
        Task<bool> StopRecordingAsync();
    }
}
