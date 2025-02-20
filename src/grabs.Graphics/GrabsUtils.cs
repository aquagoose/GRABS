using System.Runtime.CompilerServices;

namespace grabs.Graphics;

public static class GrabsUtils
{
    public static bool IsSrgb(this Format format)
    {
        switch (format)
        {
            case Format.R8G8B8A8_UNorm_SRGB:
            case Format.B8G8R8A8_UNorm_SRGB:
            case Format.BC1_UNorm_SRGB:
            case Format.BC2_UNorm_SRGB:
            case Format.BC3_UNorm_SRGB:
            case Format.BC7_UNorm_SRGB:
                return true;
            default:
                return false;
        }
    }

    public static unsafe void CopyData<T>(nint dataPtr, in ReadOnlySpan<T> data) where T : unmanaged
    {
        uint dataSize = (uint) (data.Length * sizeof(T));
        
        fixed (void* pData = data)
            Unsafe.CopyBlock((void*) dataPtr, pData, dataSize);
    }
}