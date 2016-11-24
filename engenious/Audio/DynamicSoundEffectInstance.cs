using System;
using OpenTK.Audio.OpenAL;


namespace engenious.Audio
{
    public class DynamicSoundEffectInstance : SoundEffectInstance
    {
        private int _pendingBufferCount;
        public event EventHandler BufferNeeded;
        private const int MinimumBufferCheck = 2;

        public DynamicSoundEffectInstance()
        {
            _pendingBufferCount = 0;
        }

        public override void Play()
        {
            if (State != SoundState.Stopped)
                return;

            for (int i = 0; i < MinimumBufferCheck && BufferNeeded != null; i++)
            {
                BufferNeeded(this, EventArgs.Empty);
            }

            base.Play();
        }

        public void Update()
        {
            int processedBuffers;
            AL.GetSource(sid, ALGetSourcei.BuffersProcessed, out processedBuffers);
            if (processedBuffers == 0)
                return;

            SoundSourceManager.Instance.Enqueue(AL.SourceUnqueueBuffers(sid, processedBuffers));
            _pendingBufferCount -= processedBuffers;


            BufferNeeded?.Invoke(this, EventArgs.Empty);

            for (int i = MinimumBufferCheck - _pendingBufferCount; (i > 0) && BufferNeeded != null; i--)
            {
                BufferNeeded(this, EventArgs.Empty);
            }
        }

        public void SubmitBuffer(byte[] xy)
        {
        }
    }
}