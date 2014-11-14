//-----------------------------------------------------------------------------
// VuforiaAdapter.m
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#import "VuforiaAdapter.h"
#import "string.h"
#import <QCAR/QCAR.h>
#import <QCAR/QCAR_iOS.h>
#import <QCAR/TrackerManager.h>
#import <QCAR/Tracker.h>
#import <QCAR/TrackableResult.h>
#import <QCAR/ImageTracker.h>
#import <QCAR/DataSet.h>
#import <QCAR/CameraDevice.h>
#import <QCAR/Renderer.h>
#import <QCAR/Tool.h>
#import <QCAR/VideoBackgroundConfig.h>
#import <OpenGLES/ES2/gl.h>

int gFrameWidth;
int gFrameHeight;
float gWidthScale;
float gHeightScale;
QCAR_State gState = QCAR_State::QCAR_STOPPED;

void QCAR_configureVideoBackground(int frameWidth, int frameHeight);

// QCAR State
QCAR_State QCAR_getState()
{
    return gState;
}

// Init QCAR
bool QCAR_init()
{
    QCAR::setInitParameters(QCAR::GL_20);
    
    // QCAR::init() will return positive numbers up to 100 as it progresses towards success
    // and negative numbers for error indicators
    int initSuccess = 0;
    do {
        initSuccess = QCAR::init();
    } while (0 <= initSuccess && 100 > initSuccess);
    
    if(initSuccess != 100)
    {
        return false;
    }
    
    gState = QCAR_State::QCAR_INITIALIZED;
    
    return true;
}

// ShutDown QCAR
bool QCAR_shutDown()
{

    if (gState == QCAR_State::QCAR_TRACKING)
    {
        QCAR_stopTrack();
    }
    else if (gState == QCAR_State::QCAR_STOPPED)
    {
        return false;
    }
    
    // shutdown QCAR
    QCAR::deinit();
    
    gState = QCAR_State::QCAR_STOPPED;
    return true;
}

// Set camera orientation
void QCAR_setOrientation(QCAR_Orientation orientation)
{
    if(gState != QCAR_State::QCAR_TRACKING)
    {
        return;
    }
    
    QCAR::onSurfaceChanged(gFrameWidth, gFrameHeight);
    
    QCAR::IOS_INIT_FLAGS orientationFlag;
    
    switch (orientation) {
        case QCAR_ORIENTATION_PORTRAIT:
            orientationFlag = QCAR::ROTATE_IOS_90;
            break;
        case QCAR_ORIENTATION_PORTRAIT_UPSIDEDOWN:
            orientationFlag = QCAR::ROTATE_IOS_270;
            break;
        case QCAR_ORIENTATION_LANDSCAPE_LEFT:
            orientationFlag = QCAR::ROTATE_IOS_180;
            break;
        default:
            orientationFlag = QCAR::ROTATE_IOS_0;
            break;
    }
    
    QCAR::setRotation(orientationFlag);
}

// Initializes the QCAR tracking with a datase
int QCAR_initialize(const char* dataSetPath, bool extendedTracking)
{
    // If QCAR is not in initialized state...
    if (gState != QCAR_State::QCAR_INITIALIZED)
    {
        return 200;
    }
    
    // Get the image tracker:
    QCAR::TrackerManager& trackerManager = QCAR::TrackerManager::getInstance();
    QCAR::ImageTracker* imageTracker = static_cast<QCAR::ImageTracker*> (trackerManager.initTracker(QCAR::ImageTracker::getClassType()));
    
    if (imageTracker == NULL)
    {
        printf("Failed to load tracking data set because the ImageTracker has"
            " not been initialized.");
        return 100;
    }
    // Create the data sets:
    QCAR::DataSet* dataSet = imageTracker->createDataSet();
    if (dataSet == 0)
    {
        printf("Failed to create a new tracking data.");
        return 101;
    }
    
    // Load the data sets:
    if (!dataSet->load(dataSetPath, QCAR::DataSet::STORAGE_APPRESOURCE))
    {
        printf("Failed to load data set.");
        return 102;
    }
    
    // Activate the data set:
    if (!imageTracker->activateDataSet(dataSet))
    {
        printf("Failed to activate data set.");
        return 103;
    }
    
    for (int i = 0; i < dataSet->getNumTrackables(); i++)
    {
        QCAR::Trackable* trackable = dataSet->getTrackable(i);
        if(extendedTracking)
        {
            trackable->startExtendedTracking();
        }
        else
        {
            trackable->stopExtendedTracking();
        }
    }
    
    return 0;
}

// Start AR track
bool QCAR_startTrack(int frameWidth, int frameHeight)
{
    if(gState == QCAR_State::QCAR_TRACKING)
    {
        return true;
    }
    else if(gState == QCAR_State::QCAR_STOPPED)
    {
        return false;
    }
    
    QCAR::onSurfaceChanged(frameWidth, frameHeight);
    
    // Initialise the camera
    if (QCAR::CameraDevice::getInstance().init())
    {
        // Configure video background
        QCAR_configureVideoBackground(frameWidth, frameHeight);
        QCAR::CameraDevice::getInstance().setFocusMode(QCAR::CameraDevice::FOCUS_MODE_CONTINUOUSAUTO);

        
        // Start camera capturing
        if (QCAR::CameraDevice::getInstance().start())
        {
            // Start the tracker
            QCAR::TrackerManager& trackerManager = QCAR::TrackerManager::getInstance();
            QCAR::Tracker* tracker = trackerManager.getTracker(QCAR::ImageTracker::getClassType());
            if(tracker != 0)
            {
                tracker->start();
            }
            
        }
        
        gState = QCAR_State::QCAR_TRACKING;
        
        return true;
    }

    return false;
}

// Stop AR track
bool QCAR_stopTrack()
{
    if(gState != QCAR_State::QCAR_TRACKING)
    {
        return false;
    }
    
    // Stop the tracker:
    QCAR::TrackerManager& trackerManager = QCAR::TrackerManager::getInstance();
    QCAR::Tracker* tracker = trackerManager.getTracker(QCAR::ImageTracker::getClassType());
    if(tracker != 0)
    {
        tracker->stop();
    }
    
    QCAR::CameraDevice::getInstance().stop();
    
    gState = QCAR_State::QCAR_INITIALIZED;

    return true;
}

// Update frame
TrackResult QCAR_update()
{
    TrackResult trackResult;
    
    if(gState == QCAR_State::QCAR_TRACKING)
    {
        glClear( GL_DEPTH_BUFFER_BIT | GL_COLOR_BUFFER_BIT);
    
        QCAR::State state =  QCAR::Renderer::getInstance().begin();
        QCAR::Renderer::getInstance().drawVideoBackground();

        trackResult.isTracking = state.getNumTrackableResults() > 0;
    
        if (trackResult.isTracking)
        {
            // Get the trackable
            const QCAR::TrackableResult* result = state.getTrackableResult(0);
            const QCAR::Trackable& trackable = result->getTrackable();
            QCAR::Matrix44F modelViewMatrix = QCAR::Tool::convertPose2GLMatrix(result->getPose());
    
            strcpy(trackResult.trackName, trackable.getName());
            memcpy(&trackResult.trackPose, &modelViewMatrix, sizeof(QCAR::Matrix44F));
        }
    
        QCAR::Renderer::getInstance().end();
    }
    
    return trackResult;
}

// Get Camera projection with its near/far plane
Matrix4x4 QCAR_getCameraProjection(float nearPlane, float farPlane)
{
    if(gState != QCAR_State::QCAR_TRACKING)
    {
        return Matrix4x4();
    }
    
    // Cache the projection matrix:
    const QCAR::CameraCalibration& cameraCalibration = QCAR::CameraDevice::getInstance().getCameraCalibration();

    QCAR::Matrix44F projection = QCAR::Tool::getProjectionGL(cameraCalibration, nearPlane, farPlane);
    
    Matrix4x4 result;
    memcpy(&result, &projection, sizeof(QCAR::Matrix44F));
    result.data[0] *= gWidthScale;
    result.data[5] *= gHeightScale;
    
    return result;
}


// Configure the video background
void QCAR_configureVideoBackground(int frameWidth, int frameHeight)
{
    gFrameWidth = frameWidth;
    gFrameHeight = frameHeight;
    
    // Get the default video mode
    QCAR::CameraDevice& cameraDevice = QCAR::CameraDevice::getInstance();
    QCAR::VideoMode videoMode = cameraDevice.getVideoMode(QCAR::CameraDevice::MODE_DEFAULT);
    
    // Configure the video background
    QCAR::VideoBackgroundConfig config;
    config.mEnabled = true;
    config.mSynchronous = true;
    config.mPosition.data[0] = 0.0f;
    config.mPosition.data[1] = 0.0f;
    
    // Compare aspect ratios of video and screen.  If they are different
    // we use the full screen size while maintaining the video's aspect
    // ratio, which naturally entails some cropping of the video.
    // Note - screenRect is portrait but videoMode is always landscape,
    // which is why "width" and "height" appear to be reversed.
    float aspectRatioVideo = (float)videoMode.mWidth / (float)videoMode.mHeight;
    float aspectRatioScreen = (float)gFrameWidth / (float)gFrameHeight;
    
    
    if (aspectRatioScreen > aspectRatioVideo)
    {
        config.mSize.data[0] = gFrameWidth;
        config.mSize.data[1] = (int)(gFrameWidth / aspectRatioVideo);
        gWidthScale = 1;
        gHeightScale = gFrameWidth / (gFrameHeight * aspectRatioVideo);
    }
    else
    {
        config.mSize.data[0] = (int)(gFrameHeight * aspectRatioVideo);
        config.mSize.data[1] = gFrameHeight;
        gWidthScale = gFrameHeight / (gFrameWidth / aspectRatioVideo);
        gHeightScale = 1;
    }
    
    // Set the config
    QCAR::Renderer::getInstance().setVideoBackgroundConfig(config);
}





























