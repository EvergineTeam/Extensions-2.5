/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.

@file 
    Vuforia_UWP.h

@brief
    Header file for global Vuforia methods that are UWP specific.
===============================================================================*/

#ifndef _VUFORIA_UWP_H_
#define _VUFORIA_UWP_H_

// Include files
#include <Vuforia/System.h>

namespace Vuforia
{

/// Initializes Vuforia
/**
<b>UWP:</b> Called to initialize Vuforia.  Initialization is progressive, so this function
should be called repeatedly until it returns 100 or a negative value.
Returns an integer representing the percentage complete (negative on error).
*/
int VUFORIA_API init();

/// Sets Vuforia initialization parameters
/**
<b>UWP:</b> Called to set the Vuforia initialization parameters prior to calling Vuforia::init().
*/
void VUFORIA_API setInitParameters(const char* key);


/// Sets the current rotation to be applied to the projection and background
/**
<b>UWP:</b> Called to set any rotation on the Vuforia rendered video background and
projection matrix applied to an augmentation after an auto rotation.
This method should be called from the call-back registered with DisplayInformation->OrientationChanged.
*/
void VUFORIA_API setCurrentOrientation(Windows::Graphics::Display::DisplayOrientations orientation);


} // namespace Vuforia

#endif //_VUFORIA_UWP_H_
