using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public static class UIRouter
    {
        private static Dictionary<string, UIWindow> _routesData;
        private static Stack<string> _screensStack;
        private static string _mainScreenRoute;

        public static UIState State => _screensStack.Count switch
        {
            0 => UIState.Empty,
            1 => UIState.Root,
            2 => UIState.Primary,
            _ => UIState.Empty
        };

        public static void RegistrateWindow(UIWindow window)
        {
            if (window.URL.Length > 0)
            {
                _routesData[window.URL] = window;
            }
        }
        public static void DeleteWindow(UIWindow window)
        {
            _routesData.Remove(window.URL);
        }

        public static void OpenUrl(string route)
        {
            var payload = ParseParams(route, out route);
            if (_routesData.ContainsKey(route))
            {
                var obj = _routesData.GetValueOrDefault(route);
                if (obj != null)
                {
                    obj.gameObject.SetActive(true);
                    obj.Open(payload);
                    _screensStack.Push(route);
                }
                else
                    Debug.LogError($"There no object of type:{obj.GetType().Name}");
            }
            else
                Debug.LogError($"There no route with name: {route}");
        }

        public static void HideAllWindows()
        {
            while (_screensStack.Count > 0)
                HideUrl(_screensStack.Peek());
        }

        public static void HideCurrentScreen()
        {
            if (_mainScreenRoute != null)
                HideUrl(_mainScreenRoute);
        }

        public static void HideUrl(string route)
        {
            var payload = ParseParams(route, out route);
            if (_routesData.ContainsKey(route))
            {
                CloseUpTo(route);
            }
            else
                Debug.LogError($"There no route with name: {route}");
        }

        private static void CloseUpTo(string route)
        {
            string next;
            do
            {
                next = _screensStack.Pop();
                var obj = _routesData.GetValueOrDefault(next);
                if (obj != null)
                    obj.Close();
                else
                    Debug.LogError($"There no object of type:{obj.GetType().Name}");
            } while (next != route);
        }

        public static void SwitchUrl(string route)
        {
            var payload = ParseParams(route, out route);
            if (_routesData.ContainsKey(route))
            {
                var obj = _routesData.GetValueOrDefault(route);
                if (obj != null)
                {
                    if (obj.isActiveAndEnabled)
                        CloseUpTo(route);
                    else
                    {
                        obj.Open(payload);
                        _screensStack.Push(route);
                    }
                }
                else
                    Debug.LogError($"There no object of type:{obj.GetType().Name}");
            }
            else
                Debug.LogError($"There no route with name: {route}");
        }

        public static void ReleaseLastScreen()
        {
            if (_screensStack.Count > 0)
                _screensStack.Pop();
        }

        public static void SetMainScreenRoute(string route)
        {
            _mainScreenRoute = route;
        }

        public static void ProceedBack()
        {
            if (_screensStack.Count > 0)
            {
                HideUrl(_screensStack.Peek());
                if (_screensStack.Count > 0)
                {
                    if (_routesData[_screensStack.Peek()].isActiveAndEnabled) return;
                    var prevScreen = _screensStack.Pop();
                    OpenUrl(prevScreen);
                }
                else
                    OpenUrl(_mainScreenRoute);
            }
            else
                OpenUrl(_mainScreenRoute);
        }

        private static Dictionary<string, string> ParseParams(string fullRoute, out string route)
        {
            var routeEnd = fullRoute.Split("/")[^1];
            var result  = new Dictionary<string, string>();
            var routeParams = routeEnd.Split("?");
            if (routeParams.Length > 1)
            {
                var parameters = routeParams[1].Split("&");
                if (parameters.Length > 0)
                {
                    foreach (var param in parameters)
                    {
                        var data = param.Split("=");
                        if (data.Length > 1)
                            result[data[0]] = data[1];
                    }
                }
                route = fullRoute.Split("?")[0];
            }
            else
                route = fullRoute;

            return result;
        }

        static UIRouter()
        {
            _screensStack = new Stack<string>();
            _routesData = new Dictionary<string, UIWindow>();
        }
    }

    public enum UIState
    {
        Empty,
        Root,
        Primary,
        Additional
    }
}
