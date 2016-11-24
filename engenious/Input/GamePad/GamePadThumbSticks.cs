using System;

namespace engenious.Input
{
    public struct GamePadThumbSticks : IEquatable<GamePadThumbSticks>
    {
        private const float ConversionFactor = 1.0f / short.MaxValue;
        private readonly short _leftX;
        private readonly short _leftY;
        private readonly short _rightX;
        private readonly short _rightY;

        private const short LeftThumbDeadZone = 7864; //0.24f * short.MaxValue;//MonoGame
        private const short RightThumbDeadZone = 8683;
        //
        // Properties
        //
        public Vector2 Left => new Vector2((float) _leftX * ConversionFactor, (float) _leftY * ConversionFactor);

        public Vector2 Right => new Vector2((float) _rightX * ConversionFactor, (float) _rightY * ConversionFactor);

        //
        // Constructors
        //
        internal GamePadThumbSticks(short leftX, short leftY, short rightX, short rightY)
        {
            _leftX = ExcludeAxisDeadZone(leftX, LeftThumbDeadZone); //TODO: circular dead zone?
            _leftY = ExcludeAxisDeadZone(leftY, LeftThumbDeadZone);
            _rightX = ExcludeAxisDeadZone(rightX, RightThumbDeadZone);
            _rightY = ExcludeAxisDeadZone(rightY, RightThumbDeadZone);
        }

        private static short ExcludeAxisDeadZone(short value, short deadZone)
        {
            if (value < -deadZone)
                value += deadZone;
            else if (value > deadZone)
                value -= deadZone;
            else
                return 0;
            return (short) (value / (short.MaxValue - deadZone));
        }

        //
        // Methods
        //
        public override bool Equals(object obj)
        {
            return obj is GamePadThumbSticks && Equals((GamePadThumbSticks) obj);
        }

        public bool Equals(GamePadThumbSticks other)
        {
            return _leftX == other._leftX && _leftY == other._leftY && _rightX == other._rightX &&
                   _rightY == other._rightY;
        }

        public override int GetHashCode()
        {
            return _leftX.GetHashCode() ^ _leftY.GetHashCode() ^ _rightX.GetHashCode() ^ _rightY.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Left: ({Left.X:f4}; {Left.Y:f4}); Right: ({Right.X:f4}; {Right.Y:f4})}}";
        }

        //
        // Operators
        //
        public static bool operator ==(GamePadThumbSticks left, GamePadThumbSticks right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GamePadThumbSticks left, GamePadThumbSticks right)
        {
            return !left.Equals(right);
        }
    }
}