﻿using System;

namespace grabs.Graphics;

public static class GrabsUtils
{
    public static uint BitsPerPixel(this Format format)
    {
        switch (format)
        {
            case Format.R8_UNorm:
            case Format.R8_UInt:
            case Format.R8_SNorm:
            case Format.R8_SInt:
            case Format.A8_UNorm:
                return 8;
            
            case Format.R8G8_UNorm:
            case Format.R8G8_UInt:
            case Format.R8G8_SNorm:
            case Format.R8G8_SInt:
                return 16;
            
            case Format.R16_Float:
            case Format.D16_UNorm:
            case Format.R16_UNorm:
            case Format.R16_UInt:
            case Format.R16_SNorm:
            case Format.R16_SInt:
                return 16;
            
            case Format.B5G6R5_UNorm:
            case Format.B5G5R5A1_UNorm:
                return 16;
            
            case Format.R10G10B10A2_UNorm:
            case Format.R10G10B10A2_UInt:
            case Format.R11G11B10_Float:
                return 32;
            
            case Format.R8G8B8A8_UNorm:
            case Format.R8G8B8A8_UNorm_SRGB:
            case Format.R8G8B8A8_UInt:
            case Format.R8G8B8A8_SNorm:
            case Format.R8G8B8A8_SInt:
            case Format.B8G8R8A8_UNorm:
            case Format.B8G8R8A8_UNorm_SRGB:
                return 32;
            
            case Format.R16G16_Float:
            case Format.R16G16_UNorm:
            case Format.R16G16_UInt:
            case Format.R16G16_SNorm:
            case Format.R16G16_SInt:
                return 32;
            
            case Format.R32_Float:
            case Format.R32_UInt:
            case Format.R32_SInt:
                return 32;
            
            case Format.D24_UNorm_S8_UInt:
            case Format.D32_Float:
                return 32;
            
            case Format.R16G16B16A16_Float:
            case Format.R16G16B16A16_UNorm:
            case Format.R16G16B16A16_UInt:
            case Format.R16G16B16A16_SNorm:
            case Format.R16G16B16A16_SInt:
                return 64;
            
            
            case Format.R32G32_Float:
            case Format.R32G32_UInt:
            case Format.R32G32_SInt:
                return 64;
            
            case Format.R32G32B32_Float:
            case Format.R32G32B32_UInt:
            case Format.R32G32B32_SInt:
                return 96;
            
            case Format.R32G32B32A32_Float:
            case Format.R32G32B32A32_UInt:
            case Format.R32G32B32A32_SInt:
                return 128;
            
            case Format.BC1_UNorm:
            case Format.BC1_UNorm_SRGB:
            case Format.BC4_UNorm:
            case Format.BC4_SNorm:
                return 4;
            
            case Format.BC2_UNorm:
            case Format.BC2_UNorm_SRGB:
            case Format.BC3_UNorm:
            case Format.BC3_UNorm_SRGB:
            case Format.BC5_UNorm:
            case Format.BC5_SNorm:
            case Format.BC6H_UF16:
            case Format.BC6H_SF16:
            case Format.BC7_UNorm:
            case Format.BC7_UNorm_SRGB:
                return 8;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    public static uint CalculatePitch(Format format, uint width)
    {
        uint bpp = format.BitsPerPixel();

        if (format is >= Format.BC1_UNorm and <= Format.BC7_UNorm_SRGB)
            return (bpp / 2) * width;

        return (bpp / 8) * width;
    }
}