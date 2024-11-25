﻿#include "D3D11Device.h"
#include "D3D11Utils.h"
#include "D3D11Swapchain.h"
#include "D3D11CommandList.h"
#include "D3D11ShaderModule.h"
#include "../Common.h"

namespace grabs::D3D11
{
    D3D11Device::D3D11Device(IDXGIFactory1* factory, IDXGIAdapter1* adapter, bool debug)
    {
        Factory = factory;

        UINT flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
        if (debug)
            flags |= D3D11_CREATE_DEVICE_DEBUG;

        constexpr auto featureLevel = D3D_FEATURE_LEVEL_11_1;

        D3D11_CHECK_RESULT(D3D11CreateDevice(adapter, D3D_DRIVER_TYPE_UNKNOWN, nullptr, flags, &featureLevel, 1, D3D11_SDK_VERSION, &Device, nullptr, &Context));
    }

    D3D11Device::~D3D11Device()
    {
        Context->Release();
        Device->Release();
    }

    std::unique_ptr<Swapchain> D3D11Device::CreateSwapchain(const SwapchainDescription& description, Surface* surface)
    {
        const auto dxgiSurface = dynamic_cast<DXGISurface*>(surface);

        return std::make_unique<D3D11Swapchain>(Factory, Device, dxgiSurface, description);
    }

    std::unique_ptr<CommandList> D3D11Device::CreateCommandList()
    {
        return std::make_unique<D3D11CommandList>(Device);
    }

    std::unique_ptr<Buffer> D3D11Device::CreateBuffer(const BufferDescription& description, void* data)
    {
        return std::make_unique<D3D11Buffer>(Device, description, data);
    }

    std::unique_ptr<ShaderModule> D3D11Device::CreateShaderModule(const ShaderModuleDescription& description)
    {
        return std::make_unique<D3D11ShaderModule>(description);
    }

    void D3D11Device::SubmitCommandList(CommandList* list)
    {
        GS_NULL_CHECK(list);

        const auto d3dList = dynamic_cast<D3D11CommandList*>(list);

        GS_NULL_CHECK(d3dList->CommandList);
        Context->ExecuteCommandList(d3dList->CommandList, false);
    }
}
