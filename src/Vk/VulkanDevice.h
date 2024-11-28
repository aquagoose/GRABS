﻿#pragma once

#include <vulkan/vulkan.h>

#include "grabs/Device.h"
#include "VulkanSurface.h"

namespace grabs::Vk
{
    class VulkanDevice final : public Device
    {
    public:
        VkInstance Instance;
        VkPhysicalDevice PhysicalDevice;
        VkDevice Device;

        uint32_t GraphicsQueueIndex;
        uint32_t PresentQueueIndex;

        VkQueue GraphicsQueue;
        VkQueue PresentQueue;

        VkCommandPool CommandPool;

        VulkanDevice(VkInstance instance, VulkanSurface* surface, uint32_t adapterIndex);
        ~VulkanDevice() override;

        std::unique_ptr<Swapchain> CreateSwapchain(const SwapchainDescription& description, Surface* surface) override;
        std::unique_ptr<CommandList> CreateCommandList() override;
        std::unique_ptr<Buffer> CreateBuffer(const BufferDescription& description, void* data) override;
        std::unique_ptr<ShaderModule> CreateShaderModule(const ShaderModuleDescription& description) override;
        std::unique_ptr<Pipeline> CreatePipeline(const PipelineDescription& description) override;

        void SubmitCommandList(CommandList* list) override;
    };
}
