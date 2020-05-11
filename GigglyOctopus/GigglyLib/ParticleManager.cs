namespace GigglyLib
{
    public static class ParticleManager
    {
        const int InitialPoolCapacity = 32;

        public static ulong EndIndex = 0;

        public static Particle[] Particles = new Particle[InitialPoolCapacity * 64];

        public static int CreateParticle(string texture, float x = 0, float y = 0, float deltaRotation = 0, float velocity = 0, float scale = 1, float depth = 1, float transparency = 0, float rotation = 0)
        {
            Particle particle = new Particle
            {
                X = x,
                Y = y,
                Texture = texture,
                DeltaRotation = deltaRotation,
                Velocity = velocity,
                Scale = scale,
                Depth = depth,
                Transparency = transparency,
                Rotation = rotation,
            };

            Particles[EndIndex] = particle;
            EndIndex++;

            if (EndIndex >= (ulong)Particles.Length)
                ExpandParticleSet();

            return (int)EndIndex - 1;
        }

        public static void ExpandParticleSet()
        {
            Particle[] newParticles = new Particle[Particles.Length * 2];
            Particles.CopyTo(newParticles, 0);
            Particles = newParticles;
        }

        public static void FreeParticle(ulong id)
        {
            Particles[id] = Particles[EndIndex-1];
            EndIndex--;
        }
    }

    public struct Particle
    {
        public float X;
        public float Y;
        public string Texture;
        public float DeltaRotation;
        public float Velocity;
        public float Scale;
        public float Depth;
        public float Transparency;
        public float Rotation;
    }
}
