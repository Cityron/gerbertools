#ifndef PLATFORM_HPP
#define PLATFORM_HPP

#if defined(_WIN32)
#ifdef GERBERTOOLSWRAPPER_EXPORTS
#define GERBERTOOLS_API __declspec(dllexport)
#else
#define GERBERTOOLS_API __declspec(dllimport)
#endif
#else
#define GERBERTOOLS_API __attribute__((visibility("default")))
#endif

#endif // PLATFORM_HPP