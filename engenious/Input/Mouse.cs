﻿using System;

namespace engenious.Input
{
    public static class Mouse
    {
        private static OpenTK.GameWindow _window;
        private static float _deltaPrecise = 0;

        static Mouse()
        {
            if (!WrappingHelper.ValidateStructs<OpenTK.Input.MouseState, MouseState>())
                throw new Exception("test");
        }

        internal static void UpdateWindow(OpenTK.GameWindow window)
        {
            _window = window;
            window.MouseWheel +=
                delegate(object sender, OpenTK.Input.MouseWheelEventArgs e) { _deltaPrecise -= (e.DeltaPrecise); };
        }

        public static MouseState GetState(int index)
        {
            return GetState();
            //TODO multiple mice
            /*var state = OpenTK.Input.Mouse.GetState(index);
            var actual = *(MouseState*)(&state);
            
            actual.X = _window.Mouse.X;
            actual.Y = _window.Mouse.Y;
            return actual;*/
        }

        public static unsafe MouseState GetState()
        {
            var state = OpenTK.Input.Mouse.GetState();
            var actual = *(MouseState*) (&state);
            actual.X = _window.Mouse.X;
            actual.Y = _window.Mouse.Y;
            actual.Scroll = new MouseScroll(actual.Scroll.X, _deltaPrecise); //.Y = ;
            //deltaPrecise = 0;
            return actual;
        }

        public static void SetPosition(double x, double y)
        {
            var pt = _window.PointToScreen(new System.Drawing.Point((int) x, (int) y));
            x -= (int) x;
            y -= (int) y;
            OpenTK.Input.Mouse.SetPosition(pt.X + x, pt.Y + y);
        }
    }
}