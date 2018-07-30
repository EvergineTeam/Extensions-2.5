//-----------------------------------------------------------------------------
// VuforiaAdapter.cpp
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "VuforiaAdapter.h"

int gFrameWidth;
int gFrameHeight;
QCAR_State gState = QCAR_State::QCAR_STOPPED;

Vuforia::DataSet* currentDataset;

void ConfigureVideoBackground(bool isPortrait);
void PrintInitError(int errorCode);
bool InternalStartTracking();

// QCAR State
QCAR_State QCAR_getState()
{
	return gState;
}

void QCAR_getVideoMesh(VideoMesh* mesh)
{
}

void QCAR_getVideoInfo(int* textureWidth, int* textureHeight, VideoMesh* videoMesh)
{
	if (gState != QCAR_State::QCAR_STOPPED)
	{
		auto renderingPrimitives = new Vuforia::RenderingPrimitives(Vuforia::Device::getInstance().getRenderingPrimitives());
		const Vuforia::Vec2I texSize = renderingPrimitives->getVideoBackgroundTextureSize();

		// Initialize the video background mesh
		const Vuforia::Mesh &vbMesh = renderingPrimitives->getVideoBackgroundMesh(Vuforia::VIEW_SINGULAR);
		const Vuforia::Vec3F *vbVertices = vbMesh.getPositions();
		const Vuforia::Vec2F *vbTexCoords = vbMesh.getUVs();
		const unsigned short *vbIndices = vbMesh.getTriangles();

		*textureWidth = texSize.data[0];
		*textureHeight = texSize.data[1];

		// Vertex poositions
		videoMesh->v1.posX = vbVertices[0].data[0];
		videoMesh->v1.posY = vbVertices[0].data[1];
		videoMesh->v1.posZ = vbVertices[0].data[2];
		videoMesh->v1.texCoordX = vbTexCoords[0].data[0];
		videoMesh->v1.texCoordY = vbTexCoords[0].data[1];

		videoMesh->v2.posX = vbVertices[1].data[0];
		videoMesh->v2.posY = vbVertices[1].data[1];
		videoMesh->v2.posZ = vbVertices[1].data[2];
		videoMesh->v2.texCoordX = vbTexCoords[1].data[0];
		videoMesh->v2.texCoordY = vbTexCoords[1].data[1];

		videoMesh->v3.posX = vbVertices[2].data[0];
		videoMesh->v3.posY = vbVertices[2].data[1];
		videoMesh->v3.posZ = vbVertices[2].data[2];
		videoMesh->v3.texCoordX = vbTexCoords[2].data[0];
		videoMesh->v3.texCoordY = vbTexCoords[2].data[1];

		videoMesh->v4.posX = vbVertices[3].data[0];
		videoMesh->v4.posY = vbVertices[3].data[1];
		videoMesh->v4.posZ = vbVertices[3].data[2];
		videoMesh->v4.texCoordX = vbTexCoords[3].data[0];
		videoMesh->v4.texCoordY = vbTexCoords[3].data[1];
		
		videoMesh->indices[0] = vbIndices[0];
		videoMesh->indices[1] = vbIndices[1];
		videoMesh->indices[2] = vbIndices[2];
		videoMesh->indices[3] = vbIndices[3];
		videoMesh->indices[4] = vbIndices[4];
		videoMesh->indices[5] = vbIndices[5];
		
		delete renderingPrimitives;
		renderingPrimitives = nullptr;
	}
}

#ifdef DX11
void QCAR_setVideoTexture(ID3D11Texture2D* texture)
{
	Vuforia::Renderer &vuforiaRenderer = Vuforia::Renderer::getInstance();
	vuforiaRenderer.setVideoBackgroundTexture(Vuforia::DXTextureData(texture));
}

void QCAR_updateVideoTexture(ID3D11Device* device)
{
	Vuforia::Renderer &vuforiaRenderer = Vuforia::Renderer::getInstance();
	Vuforia::DXRenderData dxRenderData(device);

	vuforiaRenderer.begin(&dxRenderData);
	vuforiaRenderer.updateVideoBackgroundTexture(nullptr);
	vuforiaRenderer.end();
};

bool QCAR_setHolographicAppCS(void* appSpecifiedCS)
{
	return Vuforia::setHolographicAppCS(appSpecifiedCS);
}
#endif

#ifdef OPENGL
void QCAR_setVideoTexture(int textureId)
{
	Vuforia::Renderer &vuforiaRenderer = Vuforia::Renderer::getInstance();
	vuforiaRenderer.setVideoBackgroundTexture(Vuforia::GLTextureData(textureId));
}

void QCAR_updateVideoTexture()
{
	Vuforia::Renderer &vuforiaRenderer = Vuforia::Renderer::getInstance();

	vuforiaRenderer.begin();
	vuforiaRenderer.updateVideoBackgroundTexture(nullptr);
	vuforiaRenderer.end();
};
#endif

// Init QCAR
#if ANDROID
void QCAR_setInitState()
{
	// Initialized through Java binding
	gState = QCAR_State::QCAR_INITIALIZED;
}
#else
bool InternalInit()
{
	// Vuforia::init() will return positive numbers up to 100 as it progresses towards success
	// and negative numbers for error indicators
	int progress = 0;
	while (progress >= 0 && progress < 100)
	{
		progress = Vuforia::init();
	}

	if (progress < 0)
	{
		PrintInitError(progress);
		return false;
	}

	gState = QCAR_State::QCAR_INITIALIZED;

	return true;
}

void QCAR_init(const char* licenseKey, InitCallback callback)
{
#if UWP
	Vuforia::setInitParameters(licenseKey);
	
	Concurrency::create_task([callback]()
	{
		bool result = InternalInit();
		callback(result);
	});
#else
	Vuforia::setInitParameters(Vuforia::GL_20, licenseKey);

	bool result = InternalInit();
	callback(result);
#endif
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
void QCAR_setOrientation(int frameWidth, int frameHeight, QCAR_Orientation orientation)
{
	if (gState != QCAR_State::QCAR_TRACKING)
	{
		return;
	}

#if IOS
	Vuforia::IOS_INIT_FLAGS orientationFlag;

	switch (orientation) {
	case QCAR_ORIENTATION_PORTRAIT:
		orientationFlag = Vuforia::ROTATE_IOS_90;
		break;
	case QCAR_ORIENTATION_PORTRAIT_UPSIDEDOWN:
		orientationFlag = Vuforia::ROTATE_IOS_270;
		break;
	case QCAR_ORIENTATION_LANDSCAPE_LEFT:
		orientationFlag = Vuforia::ROTATE_IOS_0;
		break;
	default:
		orientationFlag = Vuforia::ROTATE_IOS_180;
		break;
	}

	Vuforia::setRotation(orientationFlag);
#elif UWP
	DisplayOrientations orientationFlag;

	switch (orientation) {
	case QCAR_ORIENTATION_PORTRAIT:
		orientationFlag = DisplayOrientations::Portrait;
		break;
	case QCAR_ORIENTATION_PORTRAIT_UPSIDEDOWN:
		orientationFlag = DisplayOrientations::PortraitFlipped;
		break;
	case QCAR_ORIENTATION_LANDSCAPE_LEFT:
		orientationFlag = DisplayOrientations::Landscape;
		break;
	default:
		orientationFlag = DisplayOrientations::LandscapeFlipped;
		break;
	}

	Vuforia::setCurrentOrientation(orientationFlag);
#endif

	gFrameWidth = frameWidth;
	gFrameHeight = frameHeight;

	bool isPortrait =
		(orientation == QCAR_ORIENTATION_PORTRAIT) ||
		(orientation == QCAR_ORIENTATION_PORTRAIT_UPSIDEDOWN);


	Vuforia::onSurfaceChanged(gFrameWidth, gFrameHeight);
	
	ConfigureVideoBackground(isPortrait);
}

bool QCAR_setHint(unsigned int hint, int value)
{
	return Vuforia::setHint(hint, value);
}

// Initializes the QCAR tracking with a datase
int QCAR_loadDataSet(const char* dataSetPath, bool extendedTracking, LoadDataSetResult* trackables)
{
	// If QCAR is not in initialized state...
	if (gState != QCAR_State::QCAR_INITIALIZED)
	{
		LogMessage("QCAR has not been initialized.\n");
		return 200;
	}

	// Get the image tracker:
	Vuforia::TrackerManager& trackerManager = Vuforia::TrackerManager::getInstance();
	Vuforia::ObjectTracker* tracker = static_cast<Vuforia::ObjectTracker*> (trackerManager.initTracker(Vuforia::ObjectTracker::getClassType()));

	if (tracker == NULL)
	{
		LogMessage("Failed to load tracking data set because the ImageTracker has not been initialized.\n");
		return 100;
	}

	if (currentDataset != nullptr)
	{
		tracker->destroyDataSet(currentDataset);
	}

	// Create the data set:
	currentDataset = tracker->createDataSet();
	if (currentDataset == 0)
	{
		LogMessage("Failed to create a new tracking data.\n");
		return 101;
	}
	
	// Load the data set:
	if (!currentDataset->load(dataSetPath, Vuforia::STORAGE_APPRESOURCE))
	{
		LogMessage("Failed to load data set.\n");
		return 102;
	}

	// Activate the data set:
	if (!tracker->activateDataSet(currentDataset))
	{
		LogMessage("Failed to activate data set.\n");
		return 103;
	}

	trackables->NumTrackables = currentDataset->getNumTrackables();

	for (int i = 0; i < trackables->NumTrackables; i++)
	{
		Vuforia::Trackable* trackable = currentDataset->getTrackable(i);
		
		trackables->trackableResults[i].id = trackable->getId();
		strcpy(trackables->trackableResults[i].trackName, trackable->getName());
		
		if (trackable->isOfType(Vuforia::VuMarkTemplate::getClassType()))
		{
			trackables->trackableResults[i].targetType = QCAR_TargetTypes::VuMark;
		}
		else
		{
			trackables->trackableResults[i].targetType = QCAR_TargetTypes::ImageTarget;
		}

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
void QCAR_startTrack(StartTrackCallback callback)
{
#if UWP
	Concurrency::create_task([callback]()
	{
		bool result = InternalStartTracking();
		callback(result);
	});
#else
	bool result = InternalStartTracking();
	callback(result);
#endif
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

// Update
void QCAR_update(UpdateResult* updateResult)
{
	if (gState == QCAR_State::QCAR_TRACKING)
	{
		// Get the state from Vuforia and mark the beginning of a rendering section
		auto state = Vuforia::Renderer::getInstance().begin();

		updateResult->numTrackableResults = state.getNumTrackableResults();

		for (int tIdx = 0; tIdx < updateResult->numTrackableResults; tIdx++)
		{
			// Get the trackable
			const Vuforia::TrackableResult* result = state.getTrackableResult(tIdx);
			QCAR_TrackableResult* trackResult = &updateResult->trackableResults[tIdx];

			*((Vuforia::Matrix44F*)&trackResult->trackPose) = Vuforia::Tool::convertPose2GLMatrix(result->getPose());
			trackResult->status = (QCAR_TrackableResultStatus)result->getStatus();

			const Vuforia::Trackable& trackable = result->getTrackable();
			trackResult->id = trackable.getId();

			if (result->isOfType(Vuforia::VuMarkTargetResult::getClassType()))
			{
				const Vuforia::VuMarkTarget& vmTarget = (Vuforia::VuMarkTarget&)trackable;
				trackResult->templateId = vmTarget.getTemplate().getId();

				const Vuforia::InstanceId & vmId = vmTarget.getInstanceId();
				trackResult->dataType = (QCAR_VuMarkDataType)vmId.getDataType();
				trackResult->dataSize = (unsigned int)vmId.getLength();
				trackResult->numericValue = (unsigned int)vmId.getNumericValue();

				memcpy(trackResult->data, vmId.getBuffer(), vmId.getLength());
			}
			else
			{
				trackResult->templateId = trackResult->id;
			}
		}

		auto renderingPrimitives = new Vuforia::RenderingPrimitives(Vuforia::Device::getInstance().getRenderingPrimitives());

		// Get the Vuforia video-background projection matrix
		Vuforia::Matrix34F vbProjection = renderingPrimitives->getVideoBackgroundProjectionMatrix(Vuforia::VIEW::VIEW_SINGULAR, Vuforia::COORDINATE_SYSTEM_CAMERA);
		*((Vuforia::Matrix44F*)&updateResult->videoBackgroundProjection) = Vuforia::Tool::convert2GLMatrix(vbProjection);

		Vuforia::Renderer::getInstance().end();
	}
}

// Get Camera projection with its near/far plane
void QCAR_getCameraProjection(float nearPlane, float farPlane, QCAR_Matrix4x4* result)
{
	if (gState != QCAR_State::QCAR_TRACKING)
	{
		return;
	}

	auto renderingPrimitives = new Vuforia::RenderingPrimitives(Vuforia::Device::getInstance().getRenderingPrimitives());

	// Calculate the DX Projection matrix
	Vuforia::Matrix44F projection = Vuforia::Tool::convertPerspectiveProjection2GLMatrix(
		renderingPrimitives->getProjectionMatrix(Vuforia::VIEW_SINGULAR, Vuforia::COORDINATE_SYSTEM_CAMERA),
		nearPlane, farPlane);

	delete renderingPrimitives;
	renderingPrimitives = nullptr;

	memcpy(result, &projection, sizeof(Vuforia::Matrix44F));
}

// Configure the video background
void ConfigureVideoBackground(bool isPortrait)
{
	// Get the default video mode
	Vuforia::CameraDevice& cameraDevice = Vuforia::CameraDevice::getInstance();
	Vuforia::VideoMode videoMode = cameraDevice.getVideoMode(Vuforia::CameraDevice::MODE_DEFAULT);

	// Configure the video background
	Vuforia::VideoBackgroundConfig config;
	config.mEnabled = true;
	config.mPosition.data[0] = 0;
	config.mPosition.data[1] = 0;

	if (isPortrait)
	{
		config.mSize.data[0] = (int)(videoMode.mHeight * (gFrameHeight / (float)videoMode.mWidth));
		config.mSize.data[1] = (int)gFrameHeight;

		if (config.mSize.data[0] < gFrameWidth)
		{
			config.mSize.data[0] = (int)gFrameWidth;
			config.mSize.data[1] = (int)(gFrameWidth * (videoMode.mWidth / (float)videoMode.mHeight));
		}
	}
	else
	{
		config.mSize.data[0] = (int)gFrameWidth;
		config.mSize.data[1] = (int)(videoMode.mHeight * (gFrameWidth / (float)videoMode.mWidth));

		if (config.mSize.data[1] < gFrameHeight)
		{
			config.mSize.data[0] = (int)(gFrameHeight * (videoMode.mWidth / (float)videoMode.mHeight));
			config.mSize.data[1] = (int)gFrameHeight;
		}
	}

	// Set the config
	Vuforia::Renderer::getInstance().setVideoBackgroundConfig(config);
}

void PrintInitError(int errorCode)
{
	if (errorCode >= 0)
		return;// not an error, do nothing

	switch (errorCode)
	{
	case Vuforia::INIT_ERROR:
		LogMessage("Failed to initialize Vuforia.\n");
		break;
	case Vuforia::INIT_LICENSE_ERROR_INVALID_KEY:
		LogMessage("Invalid Key used. Please make sure you are using a valid Vuforia App Key\n");
		break;
	case Vuforia::INIT_LICENSE_ERROR_CANCELED_KEY:
		LogMessage("This App license key has been cancelled and may no longer be used. "
			"Please get a new license key.\n");
		break;
	case Vuforia::INIT_LICENSE_ERROR_MISSING_KEY:
		LogMessage("Vuforia App key is missing. Please get a valid key, by logging"
			" into your account at developer.vuforia.com and creating a new project\n");
		break;
	case Vuforia::INIT_LICENSE_ERROR_PRODUCT_TYPE_MISMATCH:
		LogMessage("Vuforia App key is not valid for this product."
			" Please get a valid key, by logging into your account at developer.vuforia.com and choosing "
			"the right product type during project creation\n");
		break;
	case Vuforia::INIT_LICENSE_ERROR_NO_NETWORK_TRANSIENT:
		LogMessage("Unable to contact server. Please try again later.\n");
		break;
	case Vuforia::INIT_LICENSE_ERROR_NO_NETWORK_PERMANENT:
		LogMessage("No network available. Please make sure you are connected to the internet.\n");
		break;
	case Vuforia::INIT_DEVICE_NOT_SUPPORTED:
		LogMessage("Failed to initialize Vuforia because this device is not supported.\n");
		break;
	case Vuforia::INIT_EXTERNAL_DEVICE_NOT_DETECTED:
		LogMessage("Failed to initialize Vuforia because this device is not docked with required external hardware.\n");
		break;
	case Vuforia::INIT_NO_CAMERA_ACCESS:
		LogMessage("Camera Access was denied to this App. \n"
			"When running on iOS8 devices, \n"
			"users must explicitly allow the App to access the camera.\n"
			"To restore camera access on your device, go to: \n"
			"Settings > Privacy > Camera > [This App Name] and switch it ON.\n");
		break;
	default:
		LogMessage("Vuforia init error. Unknown error.\n");
	}
}

bool InternalStartTracking()
{
	if (gState == QCAR_State::QCAR_TRACKING)
	{
		return true;
	}
	else if (gState == QCAR_State::QCAR_STOPPED)
	{
		return false;
	}

	Vuforia::Device::getInstance().setMode(Vuforia::Device::MODE_VR);

	// Initialise the camera
	if (!Vuforia::CameraDevice::getInstance().init(Vuforia::CameraDevice::CAMERA_DIRECTION_DEFAULT))
	{
		LogMessage("Failed to init camera.\n");
		return false;
	}

	if (!Vuforia::CameraDevice::getInstance().selectVideoMode(Vuforia::CameraDevice::MODE_DEFAULT))
	{
		LogMessage("Failed to set camera video mode.\n");
		return false;
	}

	// Configure video background
	ConfigureVideoBackground(false);
	Vuforia::CameraDevice::getInstance().setFocusMode(Vuforia::CameraDevice::FOCUS_MODE_CONTINUOUSAUTO);

	// Start camera capturing
	if (!Vuforia::CameraDevice::getInstance().start())
	{
		LogMessage("Failed to start camera capture.\n");
		return false;
	}

	// Start the tracker
	Vuforia::TrackerManager& trackerManager = Vuforia::TrackerManager::getInstance();
	Vuforia::Tracker* tracker = trackerManager.getTracker(Vuforia::ObjectTracker::getClassType());

	if (tracker == 0)
	{
		LogMessage("Cannot start tracker, tracker is null.\n");
		return false;
	}

	if (!tracker->start())
	{
		LogMessage("Failed to start tracker.\n");
		return false;
	}
		
	gState = QCAR_State::QCAR_TRACKING;
	return true;
}
