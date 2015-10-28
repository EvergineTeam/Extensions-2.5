using System;
using ObjCRuntime;

[assembly: LinkWith( "libVuforiaAdapter.a", LinkTarget.Arm64 | LinkTarget.ArmV7 | LinkTarget.ArmV7s, ForceLoad = true, IsCxx = true, Frameworks = "CoreMotion UIKit Foundation CoreGraphics QuartzCore OpenGLES CoreMedia SystemConfiguration AVFoundation CoreVideo Security")]

