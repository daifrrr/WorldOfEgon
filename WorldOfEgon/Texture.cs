using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;

namespace WorldOfEgon
{
    public static class Texture
    {
        public static int InitTexture(string path)
        {
            int width, height;
            var data = LoadTexture(path, out width, out height);
            var texId = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texId);
            GL.TexImage2D
            (
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                width,
                height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                data
            );
            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapS,
                (int) TextureWrapMode.ClampToBorder
            );
            GL.TexParameter
            (
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapT,
                (int) TextureWrapMode.ClampToBorder
            );
            GL.TexParameter
            (
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear
            );
            GL.TexParameter
            (
                TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear
            );
            return texId;
        }

        private static byte[] LoadTexture(string path, out int width, out int height)
        {
            var image = Image.Load(path) as Image<Rgba32>;
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            width = image.Width;
            height = image.Height;
            var tempPixels = image.GetPixelSpan().ToArray();

            var pixels = new List<byte>();
            foreach (var p in tempPixels)
            {
                pixels.Add(p.R);
                pixels.Add(p.G);
                pixels.Add(p.B);
                pixels.Add(p.A);
            }

            return pixels.ToArray();
        }
    }
}