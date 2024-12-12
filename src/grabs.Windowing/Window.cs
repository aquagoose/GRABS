﻿using System.Diagnostics.CodeAnalysis;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.Vulkan;
using grabs.Windowing.Events;
using Silk.NET.Core.Native;
using Silk.NET.SDL;
using Silk.NET.Vulkan;
using Event = Silk.NET.SDL.Event;
using EventType = Silk.NET.SDL.EventType;
using Instance = grabs.Graphics.Instance;
using SdlWindow = Silk.NET.SDL.Window;
using Surface = grabs.Graphics.Surface;
using WindowEvent = grabs.Windowing.Events.WindowEvent;

namespace grabs.Windowing;

public unsafe class Window : IWindowProvider, IDisposable
{
    private SdlWindow* _window;
    private uint _windowId;
    
    private Window(SdlWindow* window, uint windowId)
    {
        _window = window;
        _windowId = windowId;
    }

    public Surface CreateSurface(Instance instance)
    {
        switch (instance.Backend)
        {
            case Backend.Vulkan:
            {
                return new VulkanSurface(vkInstance =>
                {
                    VkNonDispatchableHandle surfaceHandle;
                    _sdl.VulkanCreateSurface(_window, new VkHandle(vkInstance.Handle), &surfaceHandle);

                    return new SurfaceKHR(surfaceHandle.Handle);
                });
            }

            case Backend.D3D11:
            {
                SysWMInfo info = new SysWMInfo();
                _sdl.GetWindowWMInfo(_window, &info);

                return new D3D11Surface(info.Info.Win.Hwnd);
            }

            case Backend.Unknown:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public string[] GetVulkanInstanceExtensions()
    {
        uint numExtensions;
        if (_sdl.VulkanGetInstanceExtensions(_window, &numExtensions, (byte**) null) == SdlBool.False)
            throw new Exception($"Failed to get instance extensions: {_sdl.GetErrorS()}");

        string[] extensions = new string[numExtensions];
        _sdl.VulkanGetInstanceExtensions(_window, &numExtensions, extensions);

        return extensions;
    }
    
    public void Dispose()
    {
        _sdl.DestroyWindow(_window);
        _windowMap.Remove(_windowId);
        
        if (_windowMap.Count == 0)
            _sdl.Quit();
    }
        
    private static Sdl _sdl;

    private static Dictionary<uint, Window> _windowMap;

    public static int WindowCount => _windowMap.Count;

    static Window()
    {
        _sdl = Sdl.GetApi();
        _windowMap = new Dictionary<uint, Window>();
    }
    
    public static Window Create(in WindowDescription description)
    {
        if (_windowMap.Count == 0)
        {
            if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
                throw new Exception($"Failed to initialize SDL: {_sdl.GetErrorS()}");
        }

        int x = description.X ?? Sdl.WindowposCentered;
        int y = description.Y ?? Sdl.WindowposCentered;

        WindowFlags flags = WindowFlags.Vulkan;

        SdlWindow* window =
            _sdl.CreateWindow(description.Title, x, y, (int) description.Width, (int) description.Height, (uint) flags);

        if (window == null)
        {
            _sdl.Quit();
            throw new Exception($"Failed to create SDL window: {_sdl.GetErrorS()}");
        }

        uint windowId = _sdl.GetWindowID(window);

        Window wnd = new Window(window, windowId);
        _windowMap.Add(windowId, wnd);
        return wnd;
    }

    public static bool PollEvent(out WindowEvent @event)
    {
        Event sdlEvent;
        while (_sdl.PollEvent(&sdlEvent) != 0)
        {
            switch ((EventType) sdlEvent.Type)
            {
                case EventType.Windowevent:
                {
                    switch ((WindowEventID) sdlEvent.Window.Event)
                    {
                        case WindowEventID.Close:
                        {
                            @event = new WindowEvent(Events.EventType.Quit, _windowMap[sdlEvent.Window.WindowID]);
                            return true;
                        }
                    }
                    
                    break;
                }
                
                default:
                    continue;
            }
        }

        @event = default;
        return false;
    }
}