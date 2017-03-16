//-----------------------------------------------------------------------------
// VuforiaAdapter.h
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#define MAX_TRACK_NAME 32

#if UWP
#define DX11
#define EXTERN extern "C" __declspec(dllexport)
#else
#define OPENGL
#define EXTERN extern "C"
#define __stdcall
#endif

#include <cmath>
#include <string.h>
#include <Vuforia/Device.h>
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

#if IOS
#include <Vuforia/Vuforia_iOS.h>
#include <Vuforia/GLRenderer.h>

#define LogMessage(message)  printf(message)
#elif ANDROID
#include <Vuforia/GLRenderer.h>
#include <android/log.h>

#define LogMessage(message)  __android_log_print(ANDROID_LOG_VERBOSE, "Vuforia", message)
#elif UWP
#include <wrl.h>
#include <Vuforia/Vuforia_UWP.h>
#include <Vuforia/DXRenderer.h>
#include <stdio.h>
#include <concrt.h>
#include <ppltasks.h>

using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Vuforia;

#define LogMessage(message)  OutputDebugStringA(message)
#define strcpy(a, b) strcpy_s(a, b)
#endif 

enum QCAR_State
{
	QCAR_STOPPED = 0,
	QCAR_INITIALIZED,
	QCAR_TRACKING
};

enum QCAR_Orientation
{
	QCAR_ORIENTATION_PORTRAIT = 0,
	QCAR_ORIENTATION_PORTRAIT_UPSIDEDOWN,
	QCAR_ORIENTATION_LANDSCAPE_LEFT,
	QCAR_ORIENTATION_LANDSCAPE_RIGHT,
};

/// Matrix with 4 rows and 4 columns of float items
EXTERN struct Matrix4x4
{
	float data[4 * 4];   /// Array of matrix items
};

// Track result
EXTERN struct TrackResult
{
	bool isTracking;    // Is tracking an object ?
	char trackName[MAX_TRACK_NAME]; // current trackable name
	Matrix4x4 trackPose; // Track object pose
	Matrix4x4 videoBackgroundProjection; // Video background projection
};


EXTERN struct VertexProperty
{
	float posX, posY, posZ;
	float texCoordX, texCoordY;
};

EXTERN struct VideoMesh
{
	VertexProperty v1;
	VertexProperty v2;
	VertexProperty v3;
	VertexProperty v4;

	unsigned short indices[6];
};

EXTERN void QCAR_getVideoInfo(int* textureWidth, int* textureHeight, VideoMesh* videoMesh);

#ifdef DX11
EXTERN void QCAR_setVideoTexture(ID3D11Texture2D* texture);
EXTERN void QCAR_updateVideoTexture(ID3D11Device* device);
#endif

#ifdef OPENGL
EXTERN void QCAR_setVideoTexture(int textureId);
EXTERN void QCAR_updateVideoTexture();
#endif

// Init Vuforia
#if ANDROID
EXTERN void QCAR_setInitState();
#else
EXTERN typedef void(__stdcall * InitCallback)(const bool result);
EXTERN void QCAR_init(const char* licenseKey, InitCallback callback);
#endif

// Shutdown QCAR
EXTERN bool QCAR_shutDown();

// Initialize QCAR with a dataset
EXTERN int QCAR_initialize(const char* dataSetPath, bool extendedTracking);

// Get current QCAR State
EXTERN QCAR_State QCAR_getState();

// Set camera orientation
EXTERN void QCAR_setOrientation(int frameWidth, int frameHeight, QCAR_Orientation orientation);

// Start AR track
EXTERN typedef void(__stdcall * StartTrackCallback)(const bool result);
EXTERN void QCAR_startTrack(StartTrackCallback callback);

// Stop AR track
EXTERN bool QCAR_stopTrack();

// Get camera projection
EXTERN void QCAR_getCameraProjection(float nearPlane, float farPlane, Matrix4x4* result);

// Update frame
EXTERN void QCAR_update(TrackResult* trackResult);
