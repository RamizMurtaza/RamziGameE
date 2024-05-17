using SDL2;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static System.Net.Mime.MediaTypeNames;

public static class Program
{

    private const string WindowTitle = "Rami Game E";
    private const int WindowW = 800;
    private const int WindowH = 600;

    public static bool isGameRuning = true;
    static uint Fps = 0;
    public static void Main()
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"There was an issue initilizing SDL. {SDL.SDL_GetError()}");
        }

        var window = SDL.SDL_CreateWindow(WindowTitle, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, WindowW, WindowH, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        if (window == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
        }

        var renderer = SDL.SDL_CreateRenderer(window,
                                        -1,
                                        SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED
                                        //|SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC --- for VSYNC
                                        );

        if (renderer == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
        }

        SDL_Rect dstrect = new SDL_Rect();
        dstrect.h = 100;
        dstrect.w = 100;
        dstrect.x = WindowW / 2 - dstrect.w / 2;
        dstrect.y = WindowH - dstrect.h - 20;

        var path = Path.Combine(Environment.CurrentDirectory, "Media/plane.bmp");
        var gHelloWorld = SDL.SDL_LoadBMP(path);
        if (gHelloWorld == IntPtr.Zero)
        {
            Console.WriteLine($"Unable to load image plane.bmp SDL Error:  {SDL.SDL_GetError()}");
        }
        var texture = SDL.SDL_CreateTextureFromSurface(renderer, gHelloWorld);



        SDL_Rect bolt = new SDL_Rect();
        bolt.h = 100;
        bolt.w = 10;
        bolt.x = WindowW / 2 - bolt.w / 2;
        bolt.y = WindowH - bolt.h - 20;

        var boltPath = Path.Combine(Environment.CurrentDirectory, "Media/b.bmp");
        var boltGHelloWorld = SDL.SDL_LoadBMP(boltPath);
        if (boltGHelloWorld == IntPtr.Zero)
        {
            Console.WriteLine($"Unable to load image b.bmp SDL Error:  {SDL.SDL_GetError()}");
        }
        var boltTexture = SDL.SDL_CreateTextureFromSurface(renderer, boltGHelloWorld);


        var freq_ms = SDL.SDL_GetPerformanceFrequency();
        var last_time = SDL.SDL_GetPerformanceCounter();

        uint frame_counter = 0;
        double frame_timer = last_time;

        while (isGameRuning)
        {
            UInt64 current_time = SDL_GetPerformanceCounter();
            double delta = (current_time - last_time) / freq_ms * 1000.0;

            if (current_time > frame_timer + freq_ms)
            {
                Console.WriteLine($"FPS: {frame_counter}");
                frame_counter = 0;
                frame_timer = current_time;
            }


            // Clears the current render surface.
            if (SDL.SDL_RenderClear(renderer) < 0)
            {
                Console.WriteLine($"There was an issue with clearing the render surface. {SDL.SDL_GetError()}");
            }
            if (SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255) < 0)
            {
                Console.WriteLine($"There was an issue with setting the render draw color. {SDL.SDL_GetError()}");
            }


            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        isGameRuning = false;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        switch (e.key.keysym.sym)
                        {
                            case SDL_Keycode.SDLK_RIGHT:
                                if (dstrect.x < (WindowW - dstrect.w))
                                {
                                    dstrect.x += 10;
                                    bolt.x += 10;
                                }
                                break;
                            case SDL_Keycode.SDLK_LEFT:
                                if (dstrect.x > 0)
                                {
                                    dstrect.x -= 10;
                                    bolt.x -= 10;
                                }
                                
                                break;
                            case SDL_Keycode.SDLK_ESCAPE:
                                isGameRuning = false;
                                break;

                            case SDL_Keycode.SDLK_SPACE:
                                bolt.y -= 50;                               
                                break;
                        }
                        break;
                }
            }

            
            SDL.SDL_RenderCopy(renderer, boltTexture, IntPtr.Zero, ref bolt);
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref dstrect);

            SDL.SDL_RenderPresent(renderer);

            last_time = current_time;
            ++frame_counter;
        }

        // Clean up the resources that were created.
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}