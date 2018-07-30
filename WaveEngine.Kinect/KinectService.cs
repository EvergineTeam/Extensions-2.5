// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using WaveEngine.Kinect.Enums;
using WaveEngine.Common;
using System;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
#endregion

namespace WaveEngine.Kinect
{
    /// <summary>
    /// Kinect Integration Service
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Kinect")]
    public class KinectService : UpdatableService
    {
        /// <summary>
        /// The face frame features
        /// </summary>
        private const FaceFrameFeatures Features = FaceFrameFeatures.BoundingBoxInColorSpace
                                                            | FaceFrameFeatures.PointsInColorSpace
                                                            | FaceFrameFeatures.RotationOrientation
                                                            | FaceFrameFeatures.FaceEngagement
                                                            | FaceFrameFeatures.Glasses
                                                            | FaceFrameFeatures.Happy
                                                            | FaceFrameFeatures.LeftEyeClosed
                                                            | FaceFrameFeatures.RightEyeClosed
                                                            | FaceFrameFeatures.LookingAway
                                                            | FaceFrameFeatures.MouthMoved
                                                            | FaceFrameFeatures.MouthOpen;

        /// <summary>
        /// Map depth value to Byte value ratio constant
        /// </summary>
        private const float MAPDEPTHTOBYTE = 8000 / 256;

        /// <summary>
        /// Graphics device
        /// </summary>
        private GraphicsDevice graphicsDevice = WaveServices.GraphicsDevice;

        /// <summary>
        /// The kinect sensor
        /// </summary>
        private KinectSensor kinectSensor;

        /// <summary>
        /// The multi source frame reader
        /// </summary>
        private MultiSourceFrameReader multiSourceFrameReader;

        /// <summary>
        /// The color texture
        /// </summary>
        private Texture2D colorTexture;

        /// <summary>
        ///  The color data size
        /// </summary>
        private int colorPointerSize;

        /// <summary>
        /// Color Texture handler
        /// </summary>
        private IntPtr colorTexturePointer;

        private bool updateColorTexture;

        /// <summary>
        /// The depth texture
        /// </summary>
        private Texture2D depthTexture;

        /// <summary>
        /// The depth texture data
        /// </summary>
        private byte[] depthData;

        private bool updateDepthTexture;

        /// <summary>
        /// The infrared texture
        /// </summary>
        private Texture2D infraredTexture;

        /// <summary>
        /// The infrared data size
        /// </summary>
        private int infraredPointerSize;

        /// <summary>
        /// The infrared texture handler
        /// </summary>
        private IntPtr infraredTexturePointer;

        private bool updateInfraredTexture;

        /// <summary>
        /// Gets the current source.
        /// </summary>
        /// <value>
        /// The current source.
        /// </value>
        public KinectSources CurrentSource { get; internal set; }

        /// <summary>
        /// The depth minimum reliable distance
        /// </summary>
        private ushort depthMinReliableDistance = ushort.MinValue;

        /// <summary>
        /// The depth maximum reliable distance
        /// </summary>
        private ushort depthMaxReliableDistance = ushort.MaxValue;

        /// <summary>
        /// The use depth minimum reliable distance
        /// </summary>
        private bool useDepthMinReliableDistance;

        /// <summary>
        /// The use depth maximum reliable distance
        /// </summary>
        private bool useDepthMaxReliableDistance;

        /// <summary>
        /// The infrared source value maximum
        /// </summary>
        private const float InfraredSourceValueMaximum = (float)ushort.MaxValue;

        /// <summary>
        /// The infrared source scale
        /// </summary>
        private const float InfraredSourceScale = 0.75f;

        /// <summary>
        /// The infrared output value minimum
        /// </summary>
        private const float InfraredOutputValueMinimum = 0.01f;

        /// <summary>
        /// The infrared output value maximum
        /// </summary>
        private const float InfraredOutputValueMaximum = 1.0f;

        /// <summary>
        /// The face frame sources
        /// </summary>
        private FaceFrameSource[] faceFrameSources;

        /// <summary>
        /// The face frame results
        /// </summary>
        private FaceFrameResult[] faceFrameResults;

        /// <summary>
        /// The face frame readers
        /// </summary>
        private FaceFrameReader[] faceFrameReaders = null;

        #region Properties

        /// <summary>
        /// Gets the body count.
        /// </summary>
        /// <value>
        /// The body count.
        /// </value>
        [DontRenderProperty]
        public int BodyCount { get; internal set; }

        /// <summary>
        /// Gets the face frame sources.
        /// </summary>
        /// <value>
        /// The face frame sources.
        /// </value>
        [DontRenderProperty]
        public FaceFrameSource[] FaceFrameSources
        {
            get
            {
                return this.faceFrameSources;
            }
        }

        /// <summary>
        /// Gets the face frame results.
        /// </summary>
        /// <value>
        /// The face frame results.
        /// </value>
        public FaceFrameResult[] FaceFrameResults
        {
            get
            {
                return this.faceFrameResults;
            }
        }

        /// <summary>
        /// Gets or sets the mapper.
        /// </summary>
        /// <value>
        /// The mapper.
        /// </value>
        [DontRenderProperty]
        public CoordinateMapper Mapper { get; set; }

        /// <summary>
        /// Gets the color texture.
        /// </summary>
        /// <value>
        /// The color texture.
        /// </value>
        [DontRenderProperty]
        public Texture2D ColorTexture
        {
            get
            {
                return this.colorTexture;
            }
        }

        /// <summary>
        /// Gets the infrared texture.
        /// </summary>
        /// <value>
        /// The infrared texture.
        /// </value>
        [DontRenderProperty]
        public Texture2D InfraredTexture
        {
            get
            {
                return this.infraredTexture;
            }
        }

        /// <summary>
        /// Gets the depth texture.
        /// </summary>
        /// <value>
        /// The depth texture.
        /// </value>
        [DontRenderProperty]
        public Texture2D DepthTexture
        {
            get
            {
                return this.depthTexture;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is available.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is available; otherwise, <c>false</c>.
        /// </value>
        [DontRenderProperty]
        public bool IsAvailable
        {
            get
            {
                return (this.kinectSensor != null) && this.kinectSensor.IsAvailable;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use depth minimum reliable distance].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use depth minimum reliable distance]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDepthMinReliableDistance
        {
            get
            {
                return this.useDepthMinReliableDistance;
            }

            set
            {
                this.useDepthMinReliableDistance = value;
                this.depthMinReliableDistance = this.useDepthMinReliableDistance ? this.kinectSensor.DepthFrameSource.DepthMinReliableDistance : (ushort)0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use depth maximum reliable distance].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use depth maximum reliable distance]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDepthMaxReliableDistance
        {
            get
            {
                return this.useDepthMaxReliableDistance;
            }

            set
            {
                this.useDepthMaxReliableDistance = value;
                this.depthMaxReliableDistance = this.useDepthMaxReliableDistance ? this.kinectSensor.DepthFrameSource.DepthMaxReliableDistance : ushort.MaxValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is paused.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is paused; otherwise, <c>false</c>.
        /// </value>
        [DontRenderProperty]
        public bool IsPaused
        {
            get
            {
                bool isPaused = false;

                if (this.multiSourceFrameReader != null)
                {
                    isPaused = this.multiSourceFrameReader.IsPaused;
                }

                return isPaused;
            }
        }

        /// <summary>
        /// Gets or sets the bodies.
        /// </summary>
        /// <value>
        /// The bodies.
        /// </value>
        [DontRenderProperty]
        public Body[] Bodies { get; set; }

        #endregion

        /// <summary>
        /// Starts the sensor.
        /// </summary>
        /// <param name="sources">The sources. (flags)</param>
        public void StartSensor(KinectSources sources)
        {
            this.CurrentSource = sources;

            // Gets default Sensor if null
            if (this.kinectSensor == null)
            {
                this.kinectSensor = KinectSensor.GetDefault();
            }

            // Initialize body sensor
            this.BodyCount = this.kinectSensor.BodyFrameSource.BodyCount;
            this.faceFrameSources = new FaceFrameSource[this.BodyCount];
            this.faceFrameReaders = new FaceFrameReader[this.BodyCount];
            this.faceFrameResults = new FaceFrameResult[this.BodyCount];

            // Coordinate Mapper
            this.Mapper = this.kinectSensor.CoordinateMapper;

            if (this.multiSourceFrameReader == null)
            {
                var sourcesTemp = sources;
                if (sourcesTemp.HasFlag(KinectSources.Face))
                {
                    sourcesTemp -= KinectSources.Face;
                }

                this.multiSourceFrameReader = this.kinectSensor.OpenMultiSourceFrameReader((FrameSourceTypes)sourcesTemp);
                this.multiSourceFrameReader.MultiSourceFrameArrived -= this.MultiSourceFrameReaderHandler;
                this.multiSourceFrameReader.MultiSourceFrameArrived += this.MultiSourceFrameReaderHandler;
            }

            if (sources.HasFlag(KinectSources.Face))
            {
                for (int i = 0; i < this.BodyCount; i++)
                {
                    this.faceFrameSources[i] = new FaceFrameSource(this.kinectSensor, 0, Features);
                    this.faceFrameReaders[i] = this.faceFrameSources[i].OpenReader();
                    this.faceFrameReaders[i].FrameArrived -= this.KinectServiceFaceFrameArrived;
                    this.faceFrameReaders[i].FrameArrived += this.KinectServiceFaceFrameArrived;
                }
            }

            // Open Sensor
            this.kinectSensor.Open();
        }

        /// <summary>
        /// Stops the sensor.
        /// </summary>
        public void StopSensor()
        {
            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }

            // unsubscribe events
            if (this.multiSourceFrameReader != null)
            {
                this.multiSourceFrameReader.MultiSourceFrameArrived -= this.MultiSourceFrameReaderHandler;
                this.multiSourceFrameReader = null;
            }

            if (this.faceFrameReaders != null)
            {
                foreach (var faceFrameReader in this.faceFrameReaders)
                {
                    faceFrameReader.FrameArrived -= this.KinectServiceFaceFrameArrived;
                }
            }

            this.Bodies = null;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="gameTime">The elapsed game time since the last update.</param>
        public override void Update(TimeSpan gameTime)
        {
            if (this.kinectSensor.IsOpen)
            {
                if (this.updateColorTexture)
                {
                    this.graphicsDevice.Textures.SetData(this.colorTexture, this.colorTexturePointer, this.colorPointerSize);
                    this.updateColorTexture = false;
                }

                if (this.updateDepthTexture)
                {
                    this.graphicsDevice.Textures.SetData(this.depthTexture, this.depthData);
                    this.updateDepthTexture = false;
                }

                if (this.updateInfraredTexture)
                {
                    this.graphicsDevice.Textures.SetData(this.infraredTexture, this.infraredTexturePointer, this.infraredPointerSize);
                    this.updateInfraredTexture = false;
                }
            }
        }

        /// <summary>
        /// Allows to execute custom logic during the initialization of this instance.
        /// </summary>
        protected override void Initialize()
        {
            // Get default sensor
            this.kinectSensor = KinectSensor.GetDefault();

            // Initialize textures //

            // Color Texture
            var colorDescription = this.kinectSensor.ColorFrameSource.FrameDescription;
            this.colorTexture = new Texture2D()
            {
                Format = PixelFormat.R8G8B8A8,
                Usage = TextureUsage.Dynamic,
                CpuAccess = TextureCpuAccess.Write,
                Width = colorDescription.Width,
                Height = colorDescription.Height,
                Levels = 1
            };
            this.graphicsDevice.Textures.UploadTexture(this.colorTexture);
            this.colorPointerSize = colorDescription.Width * colorDescription.Height * 4;
            this.colorTexturePointer = Marshal.AllocHGlobal(this.colorPointerSize);

            // DepthTexture
            var dephtDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
            this.depthTexture = new Texture2D()
            {
                Format = PixelFormat.R8G8B8A8,
                Usage = TextureUsage.Dynamic,
                CpuAccess = TextureCpuAccess.Write,
                Width = dephtDescription.Width,
                Height = dephtDescription.Height,
                Levels = 1
            };
            this.graphicsDevice.Textures.UploadTexture(this.depthTexture);
            int depthSize = dephtDescription.Width * dephtDescription.Height * 4;
            this.depthData = new byte[depthSize];

            // InfraredTexture
            var infraredDescription = this.kinectSensor.InfraredFrameSource.FrameDescription;
            this.infraredTexture = new Texture2D()
            {
                Format = PixelFormat.R16,
                Usage = TextureUsage.Dynamic,
                CpuAccess = TextureCpuAccess.Write,
                Width = infraredDescription.Width,
                Height = infraredDescription.Height,
                Levels = 1
            };
            this.graphicsDevice.Textures.UploadTexture(this.infraredTexture);
            this.infraredPointerSize = infraredDescription.Width * infraredDescription.Height * 2;
            this.infraredTexturePointer = Marshal.AllocHGlobal(this.infraredPointerSize);
        }

        #region Face

        /// <summary>
        /// Kinects the service face frame arrived.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FaceFrameArrivedEventArgs"/> instance containing the event data.</param>
        private void KinectServiceFaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame != null)
                {
                    // get the index of the face source from the face source array
                    int index = this.GetFaceSourceIndex(faceFrame.FaceFrameSource);

                    // check if this face frame has valid face frame results
                    // store this face frame result to draw later
                    if (faceFrame.FaceFrameResult != null)
                    {
                        this.faceFrameResults[index] = faceFrame.FaceFrameResult;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the index of the face frame source
        /// </summary>
        /// <param name="faceFrameSource">the face frame source</param>
        /// <returns>the index of the face source in the face source array</returns>
        private int GetFaceSourceIndex(FaceFrameSource faceFrameSource)
        {
            int index = -1;

            for (int i = 0; i < this.BodyCount; i++)
            {
                if (this.faceFrameSources[i] == faceFrameSource)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
        #endregion

        /// <summary>
        /// Allow to execute custom logic during the finalization of this instance.
        /// </summary>
        protected override void Terminate()
        {
            this.StopSensor();

            if (this.multiSourceFrameReader != null)
            {
                this.multiSourceFrameReader.MultiSourceFrameArrived -= this.MultiSourceFrameReaderHandler;
                this.multiSourceFrameReader.Dispose();
                this.multiSourceFrameReader = null;
            }

            for (int i = 0; i < this.BodyCount; i++)
            {
                if (this.faceFrameReaders[i] != null)
                {
                    this.faceFrameReaders[i].Dispose();
                    this.faceFrameReaders[i] = null;
                }

                if (this.faceFrameSources[i] != null)
                {
                    this.faceFrameSources[i].Dispose();
                    this.faceFrameSources[i] = null;
                }
            }

            // Closes Kinect Sensor
            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        /// <summary>
        /// Multiple Source Reader Handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="multiSourceFrameArrivedEventArgs">The <see cref="MultiSourceFrameArrivedEventArgs"/> instance containing the event data.</param>
        private void MultiSourceFrameReaderHandler(object sender, MultiSourceFrameArrivedEventArgs multiSourceFrameArrivedEventArgs)
        {
            MultiSourceFrame reference = null;

            try
            {
                reference = multiSourceFrameArrivedEventArgs.FrameReference.AcquireFrame();
            }
            catch (System.Exception)
            {
                return;
            }

            // Update Color Texture
            if (this.CurrentSource.HasFlag(KinectSources.Color))
            {
                using (ColorFrame frame = reference.ColorFrameReference.AcquireFrame())
                {
                    this.ProcessColorImage(frame);
                }
            }

            // Update Depth Texture
            if (this.CurrentSource.HasFlag(KinectSources.Depth))
            {
                using (DepthFrame frame = reference.DepthFrameReference.AcquireFrame())
                {
                    this.ProcessDepthImage(frame);
                }
            }

            // Update Infrared Image
            if (this.CurrentSource.HasFlag(KinectSources.Infrared))
            {
                using (InfraredFrame frame = reference.InfraredFrameReference.AcquireFrame())
                {
                    this.ProcessInfraredFrame(frame);
                }
            }

            // Update Body Frame
            if (this.CurrentSource.HasFlag(KinectSources.Body))
            {
                using (BodyFrame frame = reference.BodyFrameReference.AcquireFrame())
                {
                    this.ProcessBodyFrame(frame);
                }
            }
        }

        #region Body/Skeleton

        /// <summary>
        /// Processes the body frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        private void ProcessBodyFrame(BodyFrame frame)
        {
            if (frame != null)
            {
                if (this.Bodies == null)
                {
                    this.Bodies = new Body[frame.BodyCount];
                }

                // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                // As long as those body objects are not disposed and not set to null in the array,
                // those body objects will be re-used.
                frame.GetAndRefreshBodyData(this.Bodies);

                if (this.CurrentSource.HasFlag(KinectSources.Face))
                {
                    // Update face detection
                    for (int i = 0; i < this.BodyCount; i++)
                    {
                        // check if a valid face is tracked in this face source
                        if (this.faceFrameSources[i] != null && !this.faceFrameSources[i].IsTrackingIdValid)
                        {
                            // check if the corresponding body is tracked
                            if (this.Bodies[i].IsTracked)
                            {
                                // update the face frame source to track this body
                                this.faceFrameSources[i].TrackingId = this.Bodies[i].TrackingId;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Color

        /// <summary>
        /// Processes the color image.
        /// </summary>
        /// <param name="frame">The frame.</param>
        private void ProcessColorImage(ColorFrame frame)
        {
            if (this.updateColorTexture)
            {
                return;
            }

            // ColorFrameSource in yuy2 format
            if (frame != null)
            {
                FrameDescription frameDescription = frame.FrameDescription;

                using (KinectBuffer buffer = frame.LockRawImageBuffer())
                {
                    // Check resolution
                    if (frameDescription.Width == this.colorTexture.Width && frameDescription.Height == this.colorTexture.Height)
                    {
                        frame.CopyConvertedFrameDataToIntPtr(this.colorTexturePointer, (uint)this.colorPointerSize, ColorImageFormat.Rgba);
                        this.updateColorTexture = true;
                    }
                }
            }
        }
        #endregion

        #region Depth

        /// <summary>
        /// Processes the Depth Image.
        /// </summary>
        /// <param name="frame">The frame.</param>
        private void ProcessDepthImage(DepthFrame frame)
        {
            if (this.updateDepthTexture)
            {
                return;
            }

            if (frame != null)
            {
                FrameDescription frameDescription = frame.FrameDescription;

                // the fastest way to process the body index data is to directly access
                // the underlying buffer
                using (KinectBuffer buffer = frame.LockImageBuffer())
                {
                    // verify data and write the color data to the display bitmap
                    if (((frameDescription.Width * frameDescription.Height) == (buffer.Size / frameDescription.BytesPerPixel)) &&
                        (frameDescription.Width == this.depthTexture.Width) && (frameDescription.Height == this.depthTexture.Height))
                    {
                        this.ProcessGrayFrameData(buffer.UnderlyingBuffer, buffer.Size, this.depthMinReliableDistance, this.depthMaxReliableDistance, frameDescription.BytesPerPixel, this.depthData);
                        this.updateDepthTexture = true;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the gray frame data.
        /// </summary>
        /// <param name="depthFrameData">The depth frame data.</param>
        /// <param name="depthFrameDataSize">Size of the depth frame data.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        /// <param name="textureData">The texture data.</param>
        private unsafe void ProcessGrayFrameData(IntPtr depthFrameData, uint depthFrameDataSize, ushort minValue, ushort maxValue, float bytesPerPixel, byte[] textureData)
        {
            if (this.updateInfraredTexture)
            {
                return;
            }

            // depth frame data is a 16 bit value
            ushort* frameData = (ushort*)depthFrameData;

            // convert depth to a visual representation
            for (int i = 0; i < (int)(depthFrameDataSize / bytesPerPixel); ++i)
            {
                // Get the depth for this pixel
                ushort data = frameData[i];

                // To convert to a byte, we're mapping the depth value to the byte range.
                // Values outside the reliable depth range are mapped to 0 (black).
                byte grayValue = (byte)(data >= minValue && data <= maxValue ? (data / MAPDEPTHTOBYTE) : 0);
                int j = i * 4;
                textureData[j] = grayValue;
                textureData[j + 1] = grayValue;
                textureData[j + 2] = grayValue;
                textureData[j + 3] = (byte)255;
            }
        }
        #endregion

        #region Infrared

        /// <summary>
        /// Processes the infrared frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        private void ProcessInfraredFrame(InfraredFrame frame)
        {
            if (frame != null)
            {
                FrameDescription frameDescription = frame.FrameDescription;

                // the fastest way to process the body index data is to directly access
                // the underlying buffer
                using (KinectBuffer buffer = frame.LockImageBuffer())
                {
                    // verify data and write the color data to the display bitmap
                    if (((frameDescription.Width * frameDescription.Height) == (buffer.Size / frameDescription.BytesPerPixel)) &&
                        (frameDescription.Width == this.infraredTexture.Width) && (frameDescription.Height == this.infraredTexture.Height))
                    {
                        frame.CopyFrameDataToIntPtr(this.infraredTexturePointer, (uint)this.infraredPointerSize);
                        this.updateInfraredTexture = true;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the infrared frame data.
        /// </summary>
        /// <param name="infraredFrameData">The infrared frame data.</param>
        /// <param name="infraredFrameDataSize">Size of the infrared frame data.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        ////private unsafe void ProcessInfraredFrameData(IntPtr infraredFrameData, uint infraredFrameDataSize, int bytesPerPixel)
        ////{
        ////    // infrared frame data is a 16 bit value
        ////    ushort* frameData = (ushort*)infraredFrameData;

        ////    // process the infrared data
        ////    for (int i = 0; i < (infraredFrameDataSize / bytesPerPixel); ++i)
        ////    {
        ////        // since we are displaying the image as a normalized grey scale image, we need to convert from
        ////        // the ushort data (as provided by the InfraredFrame) to a value from [InfraredOutputValueMinimum, InfraredOutputValueMaximum]
        ////        float infrared = Math.Min(InfraredOutputValueMaximum, (((float)frameData[i] / InfraredSourceValueMaximum * InfraredSourceScale) * (1.0f - InfraredOutputValueMinimum)) + InfraredOutputValueMinimum);
        ////        byte infraredByteValue = (byte)(infrared * 255f);
        ////        int j = i * 4;
        ////        this.infraredTextureData[0][0][j] = infraredByteValue;
        ////        this.infraredTextureData[0][0][j + 1] = infraredByteValue;
        ////        this.infraredTextureData[0][0][j + 2] = infraredByteValue;
        ////        this.infraredTextureData[0][0][j + 3] = (byte)255;
        ////    }
        ////}

        #endregion
    }
}
