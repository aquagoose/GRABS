﻿#include "VulkanSwapchain.h"

#include <algorithm>

#include "VkUtils.h"

namespace grabs::Vk {
    VulkanSwapchain::VulkanSwapchain(VkInstance instance, VkPhysicalDevice physDevice, VulkanDevice* device, const SwapchainDescription& description, VulkanSurface* surface) {
        Device = device->Device;

        VkSurfaceCapabilitiesKHR surfaceCapabilities;
        vkGetPhysicalDeviceSurfaceCapabilitiesKHR(physDevice, surface->Surface, &surfaceCapabilities);

        uint32_t minImages = std::clamp(description.NumBuffers, surfaceCapabilities.minImageCount, surfaceCapabilities.maxImageCount);

        VkFormat format = Utils::FormatToVk(description.Format);
        VkPresentModeKHR presentMode = Utils::PresentModeToVk(description.PresentMode);

        VkExtent2D extent {
            description.Size.Width,
            description.Size.Height
        };

        VkSwapchainCreateInfoKHR swapchainCreateInfo {
            .sType = VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR,
            .surface = surface->Surface,
            .minImageCount = minImages,
            .imageFormat = format,
            .imageColorSpace = VK_COLOR_SPACE_SRGB_NONLINEAR_KHR,
            .imageExtent = extent,
            .imageArrayLayers = 1,
            .imageUsage = VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT,
            .preTransform = surfaceCapabilities.currentTransform,
            .compositeAlpha = VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR,
            .presentMode = presentMode,
            .clipped = VK_FALSE,
            .oldSwapchain = nullptr
        };

        CHECK_RESULT(vkCreateSwapchainKHR(Device, &swapchainCreateInfo, nullptr, &Swapchain));

        uint32_t numImages;
        CHECK_RESULT(vkGetSwapchainImagesKHR(Device, Swapchain, &numImages, nullptr));
        std::vector<VkImage> swapchainImages(numImages);
        CHECK_RESULT(vkGetSwapchainImagesKHR(Device, Swapchain, &numImages, swapchainImages.data()));

        for (const auto image : swapchainImages) {
            VkImageViewCreateInfo info {
                .sType = VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO,
                .image = image,
                .viewType = VK_IMAGE_VIEW_TYPE_2D,
                .format = swapchainCreateInfo.imageFormat,
                .components = {
                    .r = VK_COMPONENT_SWIZZLE_IDENTITY,
                    .g = VK_COMPONENT_SWIZZLE_IDENTITY,
                    .b = VK_COMPONENT_SWIZZLE_IDENTITY,
                    .a = VK_COMPONENT_SWIZZLE_IDENTITY
                },
                .subresourceRange = {
                    .aspectMask = VK_IMAGE_ASPECT_COLOR_BIT,
                    .baseMipLevel = 0,
                    .levelCount = 1,
                    .baseArrayLayer = 0,
                    .layerCount = 1
                }
            };

            VkImageView view;
            CHECK_RESULT(vkCreateImageView(Device, &info, nullptr, &view));

            SwapchainViews.push_back(view);
        }
    }

    VulkanSwapchain::~VulkanSwapchain() {
        for (const auto view : SwapchainViews) {
            vkDestroyImageView(Device, view, nullptr);
        }

        vkDestroySwapchainKHR(Device, Swapchain, nullptr);
    }

    Texture* VulkanSwapchain::GetNextTexture() {
        return nullptr;
    }

    void VulkanSwapchain::Present() {

    }
}
