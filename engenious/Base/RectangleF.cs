﻿using System;
using OpenTK;

namespace engenious
{
    public struct RectangleF
    {
        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public float X{ get; set; }

        public float Y{ get; set; }

        public float Width{ get; set; }

        public float Height{ get; set; }

        public float Left{ get { return X; } }

        public float Right{ get { return X + Width; } }

        public float Top{ get { return Y; } }

        public float Bottom{ get { return Y + Height; } }

        public Vector2 Size
        { 
            get { return new Vector2(Width, Height); }
            set
            {
                this.Width = value.X;
                this.Height = value.Y;
            }
        }

        public Vector2 Location
        { 
            get { return new Vector2(X, Y); } 
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public bool Contains(float x, float y)
        {
            return x >= X && x < X + Width && y >= Y && y < Y + Height;
        }

        public bool Contains(Vector2 location)
        {
            return Contains(location.X, location.Y);
        }

        public bool Contains(RectangleF rect)
        {
            return Contains(rect.X, rect.Y) && Contains(rect.Right, rect.Bottom);
        }

        public void Inflate(float width, float height)
        {
            X -= width;
            Y -= height;

            Width += width;
            Height += height;
        }

        public void Inflate(Vector2 size)
        {
            Inflate(size.X, size.Y);
        }

        public void Intersect(RectangleF rect)
        {
            float x = Math.Max(X, rect.X);
            float y = Math.Max(Y, rect.Y);
            float right = Math.Max(Right, rect.Right);
            float bottom = Math.Max(Bottom, rect.Bottom);
            this.X = x;
            this.Y = y;

            this.Width = right - x;
            this.Height = bottom - y;
        }

        public bool IntersectsWith(RectangleF rect)
        {
            return (X >= rect.X && X < rect.Right || rect.X >= X && rect.X < Right) &&
            (Y >= rect.Y && Y < rect.Bottom || rect.Y >= Y && rect.Y < Bottom);
        }

        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
        }

        public void Offset(Vector2 location)
        {
            Offset(location.X, location.Y);
        }

        public override string ToString()
        {
            return string.Format("[Rectangle: X={0}, Y={1}, Width={2}, Height={3}]", X, Y, Width, Height);
        }

        public override int GetHashCode()
        {
            return (int)(X + Y + Width + Height);
        }

        public override bool Equals(object obj)
        {
            if (obj is RectangleF)
            {
                RectangleF sec = (RectangleF)obj;
                return X == sec.X && Y == sec.Y && Width == sec.Width && Height == sec.Height;
            }
            return false;
        }

        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return a.X != b.X || a.Y != b.Y || a.Width != b.Width || a.Height != b.Height;
        }

        public static RectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }

        public static RectangleF Inflate(RectangleF rect, float width, float height)
        {
            return new RectangleF(rect.X - width, rect.Y - height, rect.Width + width, rect.Height + height);
        }


        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            float x = Math.Max(a.X, b.X);
            float y = Math.Max(a.Y, b.Y);
            float right = Math.Max(a.Right, b.Right);
            float bottom = Math.Max(a.Bottom, b.Bottom);
            return RectangleF.FromLTRB(x, y, right, bottom);
        }

        public static readonly RectangleF Empty = new RectangleF(0, 0, 0, 0);
        /*public static Rectangle Ceiling(RectangleF value)
        {
        }*/
    }

}