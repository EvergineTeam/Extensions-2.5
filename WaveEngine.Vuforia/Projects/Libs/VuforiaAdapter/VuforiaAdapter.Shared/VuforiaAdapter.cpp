//-----------------------------------------------------------------------------
// VuforiaAdapter.cpp
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "VuforiaAdapter.h"
#include <string.h>
#include <Vuforia/Vuforia.h>
#include <Vuforia/TrackerManager.h>
#include <Vuforia/Tracker.h>
#include <Vuforia/TrackableResult.h>
#include <Vuforia/DataSet.h>
#include <Vuforia/CameraDevice.h>
#include <Vuforia/Renderer.h>
#include <Vuforia/Tool.h>
#include <Vuforia/VideoBackgroundConfig.h>
#include <Vuforia/ObjectTracker.h>

#if __APPLE__
#include <Vuforia/Vuforia_iOS.h>
#include <OpenGLES/ES2/gl.h>
#elif ANDROID
#include <EGL\egl.h>
#include <GLES\gl.h>
#endif

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
#if IOS
bool QCAR_init(const char* licenseKey)
{
    Vuforia::setInitParameters(Vuforia::GL_20, licenseKey);
    
	// QCAR::init() will return positive numbers up to 100 as it progresses towards success
	// and negative numbers for error indicators
	int initSuccess = 0;
	do {
		initSuccess = Vuforia::init();
	} while (0 <= initSuccess && 100 > initSuccess);

	if (initSuccess != 100)
	{
		return false;
	}
	gState = QCAR_State::QCAR_INITIALIZED;

	return true;
}
#elif ANDROID
void QCAR_setInitState()
{
	gState = QCAR_State::QCAR_INITIALIZED;
}
#endif

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
	Vuforia::deinit();

	gState = QCAR_State::QCAR_STOPPED;
	return true;
}


// Set camera orientation
void QCAR_setOrientation(QCAR_Orientation orientation)
{
	if (gState != QCAR_State::QCAR_TRACKING)
	{
		return;
	}

	Vuforia::onSurfaceChanged(gFrameWidth, gFrameHeight);

#if __APPLE__
	Vuforia::IOS_INIT_FLAGS orientationFlag;

	switch (orientation) {
	case QCAR_ORIENTATION_PORTRAIT:
		orientationFlag = Vuforia::ROTATE_IOS_90;
		break;
	case QCAR_ORIENTATION_PORTRAIT_UPSIDEDOWN:
		orientationFlag = Vuforia::ROTATE_IOS_270;
		break;
	case QCAR_ORIENTATION_LANDSCAPE_LEFT:
		orientationFlag = Vuforia::ROTATE_IOS_180;
		break;
	default:
		orientationFlag = Vuforia::ROTATE_IOS_0;
		break;
	}

	Vuforia::setRotation(orientationFlag);
#endif
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
	Vuforia::TrackerManager& trackerManager = Vuforia::TrackerManager::getInstance();
	Vuforia::ObjectTracker* tracker = static_cast<Vuforia::ObjectTracker*> (trackerManager.initTracker(Vuforia::ObjectTracker::getClassType()));

	if (tracker == NULL)
	{
		printf("Failed to load tracking data set because the ImageTracker has"
			" not been initialized.");
		return 100;
	}

	// Create the data sets:
	Vuforia::DataSet* dataSet = tracker->createDataSet();
	if (dataSet == 0)
	{
		printf("Failed to create a new tracking data.");
		return 101;
	}

	// Load the data sets:
	if (!dataSet->load(dataSetPath, Vuforia::STORAGE_APPRESOURCE))
	{
		printf("Failed to load data set.");
		return 102;
	}

	// Activate the data set:
	if (!tracker->activateDataSet(dataSet))
	{
		printf("Failed to activate data set.");
		return 103;
	}

	for (int i = 0; i < dataSet->getNumTrackables(); i++)
	{
		Vuforia::Trackable* trackable = dataSet->getTrackable(i);
		if (extendedTracking)
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
	if (gState == QCAR_State::QCAR_TRACKING)
	{
		return true;
	}
	else if (gState == QCAR_State::QCAR_STOPPED)
	{
		return false;
	}

	Vuforia::onSurfaceChanged(frameWidth, frameHeight);

	// Initialise the camera
	if (Vuforia::CameraDevice::getInstance().init())
	{
		// Configure video background
		QCAR_configureVideoBackground(frameWidth, frameHeight);
		Vuforia::CameraDevice::getInstance().setFocusMode(Vuforia::CameraDevice::FOCUS_MODE_CONTINUOUSAUTO);


		// Start camera capturing
		if (Vuforia::CameraDevice::getInstance().start())
		{
			// Start the tracker
			Vuforia::TrackerManager& trackerManager = Vuforia::TrackerManager::getInstance();
			Vuforia::Tracker* tracker = trackerManager.getTracker(Vuforia::ObjectTracker::getClassType());

			if (tracker != 0)
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
	if (gState != QCAR_State::QCAR_TRACKING)
	{
		return false;
	}

	// Stop the tracker:
	Vuforia::TrackerManager& trackerManager = Vuforia::TrackerManager::getInstance();
	Vuforia::Tracker* tracker = trackerManager.getTracker(Vuforia::ObjectTracker::getClassType());

	if (tracker != 0)
	{
		tracker->stop();
	}

	Vuforia::CameraDevice::getInstance().stop();

	gState = QCAR_State::QCAR_INITIALIZED;

	return true;
}

// Update frame
TrackResult QCAR_update()
{
	TrackResult trackResult;

	if (gState == QCAR_State::QCAR_TRACKING)
	{
		glClear(GL_DEPTH_BUFFER_BIT | GL_COLOR_BUFFER_BIT);

		Vuforia::State state = Vuforia::Renderer::getInstance().begin();
		Vuforia::Renderer::getInstance().drawVideoBackground();

		trackResult.isTracking = state.getNumTrackableResults() > 0;

		if (trackResult.isTracking)
		{
			// Get the trackable
			const Vuforia::TrackableResult* result = state.getTrackableResult(0);
			const Vuforia::Trackable& trackable = result->getTrackable();
			Vuforia::Matrix44F modelViewMatrix = Vuforia::Tool::convertPose2GLMatrix(result->getPose());

			strcpy(trackResult.trackName, trackable.getName());
			memcpy(&trackResult.trackPose, &modelViewMatrix, sizeof(Vuforia::Matrix44F));
		}

		Vuforia::Renderer::getInstance().end();
	}

	return trackResult;
}

// Get Camera projection with its near/far plane
Matrix4x4 QCAR_getCameraProjection(float nearPlane, float farPlane)
{
	if (gState != QCAR_State::QCAR_TRACKING)
	{
		return Matrix4x4();
	}

	// Cache the projection matrix:
	const Vuforia::CameraCalibration& cameraCalibration = Vuforia::CameraDevice::getInstance().getCameraCalibration();

	Vuforia::Matrix44F projection = Vuforia::Tool::getProjectionGL(cameraCalibration, nearPlane, farPlane);

	Matrix4x4 result;
	memcpy(&result, &projection, sizeof(Vuforia::Matrix44F));
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
	Vuforia::CameraDevice& cameraDevice = Vuforia::CameraDevice::getInstance();
	Vuforia::VideoMode videoMode = cameraDevice.getVideoMode(Vuforia::CameraDevice::MODE_DEFAULT);

	// Configure the video background
	Vuforia::VideoBackgroundConfig config;
	config.mEnabled = true;
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
	Vuforia::Renderer::getInstance().setVideoBackgroundConfig(config);
}



























