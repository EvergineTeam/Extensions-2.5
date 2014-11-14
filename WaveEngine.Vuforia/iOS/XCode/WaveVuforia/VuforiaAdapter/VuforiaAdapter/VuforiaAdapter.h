//-----------------------------------------------------------------------------
// VuforiaAdapter.h
//
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#define MAX_TRACK_NAME 32

extern "C"
{
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
    struct Matrix4x4
    {
        float data[4*4];   /// Array of matrix items
    };
    
    // Track result
    struct TrackResult
    {
        bool isTracking;    // Is tracking an object ?
        char trackName[MAX_TRACK_NAME]; // current trackable name
        Matrix4x4 trackPose; // Track object pose
    };
    
    // Init QCAR
    bool QCAR_init();
    
    // Shutdown QCAR
    bool QCAR_shutDown();
    
    // Initialize QCAR with a dataset
    int QCAR_initialize(const char* dataSetPath, bool extendedTracking);
    
    // Get current QCAR State
    QCAR_State QCAR_getState();
    
    // Set camera orientation
    void QCAR_setOrientation(QCAR_Orientation orientation);

    // Start AR track
    bool QCAR_startTrack(int frameWidth, int frameHeight);

    // Stop AR track
    bool QCAR_stopTrack();
    
    // Get camera projection
    Matrix4x4 QCAR_getCameraProjection(float nearPlane, float farPlane);

    // Update frame
    TrackResult QCAR_update();
}