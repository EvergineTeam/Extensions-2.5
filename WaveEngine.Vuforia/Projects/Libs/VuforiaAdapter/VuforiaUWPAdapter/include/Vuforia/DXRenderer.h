/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.

@file 
    DXRenderer.h

@brief
    Header file for DX renderer classes.
===============================================================================*/


#ifndef _VUFORIA_DXRENDERER_H_
#define _VUFORIA_DXRENDERER_H_

// Include files
#include <Vuforia/Renderer.h>
#include <wrl/client.h>
#include <d3d11.h>

namespace Vuforia 
{

/// DX-specific classes


/**
*  DXTextureData object passed to Vuforia to set the DX texture info of the video
*  background texture created by the app.
*
*  Use with Vuforia::Renderer::setVideoBackgroundTexture and in conjunction
*  with Vuforia::Renderer::updateVideoBackgroundTexture
*/
class VUFORIA_API DXTextureData : public TextureData
{
public:
    /**
    *  Arguments are a convenience that allows
    *  memberse to be set when the object is constructed.
    */
    DXTextureData(ID3D11Texture2D* texture2D);
    DXTextureData();
    ~DXTextureData();

    virtual const void* buffer() const;

    struct {
        Microsoft::WRL::ComPtr<ID3D11Texture2D> mTexture2D;
    } mData;
};

/**
*  DXRenderData object passed to Vuforia when performing DirectX rendering
*  operations. Pass a pointer to the current drawable texture and a pointer
*  to a valid render command encoder encapsulated in the mData struct.
*
*  Use with Vuforia::Renderer::begin and Vuforia::Renderer::end
*/
class VUFORIA_API DXRenderData : public RenderData
{
public:
    DXRenderData();
    DXRenderData(ID3D11Device* d3d11Device);
    ~DXRenderData();

    virtual const void* buffer() const;

    Microsoft::WRL::ComPtr<ID3D11Device> mD3D11Device;
};

} // namespace Vuforia

#endif //_VUFORIA_DXRENDERER_H_
