using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace Smallpt
{
    public static class Smallpt
    {
        public static readonly Vector3 CameraPosition = new Vector3(0, 0, -1);

        public static readonly Vector3 CameraFacingDirection = Vector3.Normalize(new Vector3(0, 0, 1));


        private enum ReflectionType
        {
            DIFFUSE,
            SPECULAR,
            REFRACTIVE
        };

        private class Sphere
        {
            public float radius;
            public Vector3 centre, emission, colour;
            public ReflectionType reflection;

            public Sphere(float radius, Vector3 centre, Vector3 emission, Vector3 colour, ReflectionType reflection)
            {
                this.radius = radius;
                this.centre = centre;
                this.emission = emission;
                this.colour = colour;
                this.reflection = reflection;
            }

            public Sphere()
            {

            }

            public float Intersect(Vector3 position, Vector3 direction)
            {
                // Solve t^2*d.d + 2*t*(o-p).d + (o-p).(o-p)-R^2 = 0
                Vector3 op = centre - position;
                const float eps = 1e-4f;

                float b = Vector3.Dot(op, direction);
                float det = b * b - Vector3.Dot(op, op) + radius * radius;

                if (det < 0)
                    return 0;

                det = (float)(Math.Sqrt(det));
                float t = b - det;

                if (t > eps)
                    return t;

                t = b + det;

                if (t > eps)
                    return t;

                return 0;
            }
        };

        static readonly Sphere[] spheres =
        {
            // Scene: radius, position, emission, color, material
            new Sphere(1e5f, new Vector3(1e5f + 1f, 40.8f, 81.6f), new Vector3(0f, 0f, 0f), new Vector3(0.75f, 0.25f, 0.25f), ReflectionType.DIFFUSE),      // Left
            new Sphere(1e5f, new Vector3(-1e5f + 99f, 40.8f, 81.6f), new Vector3(0f, 0f, 0f), new Vector3(0.25f, 0.25f, 0.75f), ReflectionType.DIFFUSE),    // Right
            new Sphere(1e5f, new Vector3(50f, 40.8f, 1e5f), new Vector3(0f, 0f, 0f), new Vector3(0.75f, 0.75f, 0.75f), ReflectionType.DIFFUSE),             // Back
            new Sphere(1e5f, new Vector3(50f, 40.8f, -1e5f + 170f), new Vector3(0f, 0f, 0f), new Vector3(), ReflectionType.DIFFUSE),                        // Front
            new Sphere(1e5f, new Vector3(50f, 1e5f, 81.6f), new Vector3(0f, 0f, 0f), new Vector3(0.75f, 0.75f, 0.75f), ReflectionType.DIFFUSE),             // Bottom
            new Sphere(1e5f, new Vector3(50f, -1e5f + 81.6f, 81.6f), new Vector3(0f, 0f, 0f), new Vector3(0.75f, 0.75f, 0.75f), ReflectionType.DIFFUSE),    // Top
            // new Sphere(16.5f, new Vector3(27f, 16.5f, 47f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f) * 0.999f, ReflectionType.SPECULAR),            // Mirror
            // new Sphere(16.5f, new Vector3(73f, 16.5f, 78f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f) * 0.999f, ReflectionType.REFRACTIVE),          // Glass
            new Sphere(600f, new Vector3(50f, 681.6f - 0.27f, 81.6f), new Vector3(12f, 12f, 12f), new Vector3(), ReflectionType.DIFFUSE)                    // Light
        };



        public static Image RayTrace(int width, int height, int numberOfSamples = 4)
        {
            Rgba32[] pixelData = new Rgba32[width * height];
            float aspectRatio = (float)(width) / (float)(height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    // Perform ray tracing for each pixel

                    float u = (float)(x) / (float)(width - 1);
                    float v = (float)(y) / (float)(height - 1);
                    int index = y * width + x;

                    Trace(aspectRatio, u, v, ref pixelData[index]);
                }
            }

            return Image<Rgba32>.LoadPixelData(pixelData, width, height);
        }



        private static void Trace(float aspectRatio, float i, float j, ref Rgba32 colour)
        {
            const float focusDistance = 0.1f;
            double theta = ((Math.PI / 180.0) * 40.0);
            float viewportHeight = 2.0f * (float)(Math.Tan(theta / 2.0));
            float viewportWidth = aspectRatio * viewportHeight;

            Vector3 u = Vector3.Normalize(Vector3.Cross(new Vector3(0, 1, 0), CameraFacingDirection));
            Vector3 horizontal = u * focusDistance * viewportWidth;
            Vector3 vertical = Vector3.Cross(CameraFacingDirection, u) * focusDistance * viewportHeight;

            Vector3 screenLowerLeftCorner = CameraPosition - horizontal / 2.0f - vertical / 2.0f - CameraFacingDirection * focusDistance;
            Vector3 screenPixelOffset = screenLowerLeftCorner + horizontal * i + vertical * j - CameraPosition;

            Vector3 rayPosition = CameraPosition + screenPixelOffset;
            Vector3 rayDirection = Vector3.Normalize(screenPixelOffset);

            Random r = new Random();
            Vector3 col = CalculateColour(rayPosition, rayDirection, 0, r) * 256;
            colour = new Rgba32(col.X, col.Y, col.Z, 1);
        }

        private static Vector3 CalculateColour(Vector3 position, Vector3 direction, int depth, Random r)
        {
            Sphere sphere = new Sphere();
            if (!GetClosestIntersectionWithSpheres(position, direction, ref sphere))
            {
                Console.WriteLine("did not intersect");
                return new Vector3(0, 0, 0);  // if miss, return black
            }

            Vector3 n = Vector3.Normalize(position - sphere.centre);
            Vector3 nl = Vector3.Dot(n, direction) < 0 ? n : n * -1;

            double maxRefl = Math.Max(sphere.colour.X, Math.Max(sphere.colour.Y, sphere.colour.Z));

            if (depth > 5)
            {
                return sphere.emission; //R.R.
            }

            if (sphere.reflection == ReflectionType.DIFFUSE)
            {
                // Ideal Refl_t::DIFFUSE reflection
                float r1 = 2 * MathF.PI * r.Next();
                float r2 = r.Next();
                float r2s = (float)(Math.Sqrt(r2));

                Vector3 w = nl;
                Vector3 u = Math.Abs(w.X) > 0.1 ? new Vector3(0, 1, 0) : Vector3.Normalize(new Vector3(1 % w.X, 1 % w.Y, 1 % w.Z));

                Vector3 v = new Vector3(w.X % u.X, w.Y % u.Y, w.Z % u.Z);
                Vector3 d = Vector3.Normalize(u * (float)(Math.Cos(r1)) * r2s + v * (float)(Math.Sin(r1)) * r2s + w * (float)(Math.Sqrt(1 - r2)));

                return sphere.emission + sphere.colour * (CalculateColour(position, direction, depth + 1, r));
            }
            else if (sphere.reflection == ReflectionType.SPECULAR)
            {
                // Ideal Refl_t::SPECULAR reflection
                //return obj.emission + f.multiply(radiance(Ray(x, r.direction - n * 2 * n.dotProduct(r.direction)), depth, Xi));
                return new Vector3(0, 0, 0);
            }
            else
            {
                // Ray reflRay(x, r.direction -n * 2 * n.dotProduct(r.direction)); // Ideal dielectric Refl_t::REFRACTION
                // bool into = n.dotProduct(nl) > 0;                // Ray from outside going in?
                // double nc = 1, nt = 1.5, nnt = into ? nc / nt : nt / nc, ddn = r.direction.dotProduct(nl), cos2t;

                // if ((cos2t = 1 - nnt * nnt * (1 - ddn * ddn)) < 0)
                // {
                //     // Total internal reflection
                //     return obj.emission + f.multiply(radiance(reflRay, depth, Xi));
                // }

                // Vector3 tdir = (r.direction * nnt - n * ((into ? 1 : -1) * (ddn * nnt + sqrt(cos2t)))).normalise();
                // double a = nt - nc, b = nt + nc, R0 = a * a / (b * b), c = 1 - (into ? -ddn : tdir.dotProduct(n));
                // double Re = R0 + (1 - R0) * c * c * c * c * c, Tr = 1 - Re, P = .25 + .5 * Re, RP = Re / P, TP = Tr / (1 - P);

                // return obj.emission + f.multiply(depth > 2 ? (erand48(Xi) < P ? // Russian roulette
                //     radiance(reflRay, depth, Xi) * RP
                //     : radiance(Ray(x, tdir), depth, Xi) * TP)
                //     : radiance(reflRay, depth, Xi) * Re + radiance(Ray(x, tdir), depth, Xi) * Tr);
                // }

                return new Vector3(0, 0, 0);

            }
        }

        private static bool GetClosestIntersectionWithSpheres(Vector3 position, Vector3 direction, ref Sphere sphereOut)
        {
            foreach (Sphere sphere in spheres)
            {
                float distance = sphere.Intersect(position, direction);
                if (distance < 1e20)
                {
                    sphereOut = sphere;
                    return true;
                }
            }

            return false;
        }




    }
}

