using System;
using System.Collections.Generic;
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenGLRefactorLater
{
    public class Mesh
    {
        public bool LoadMesh(string path, out int vao, out int vertexCount)
        {
            var importer = new AssimpContext();
            var scene = importer.ImportFile(path);
            Console.WriteLine("{0} animations", scene.AnimationCount);
            Console.WriteLine("{0} cameras", scene.CameraCount);
            Console.WriteLine("{0} lights", scene.LightCount);
            Console.WriteLine("{0} materials", scene.MaterialCount);
            Console.WriteLine("{0} meshes", scene.MeshCount);
            Console.WriteLine("{0} textures", scene.TextureCount);

            var mesh = scene.Meshes[0];
            Console.WriteLine("{0} vertices in mesh[0]", mesh.Vertices.Count);
            vertexCount = mesh.Vertices.Count;

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            var points = new List<float>();
            var normals = new List<float>();
            var texCoords = new List<float>();
            if (mesh.HasVertices)
            {
                foreach (var vp in mesh.Vertices)
                {
                    points.Add(vp.X);
                    points.Add(vp.Y);
                    points.Add(vp.Z);
                }
            }
            if (mesh.HasNormals)
            {
                foreach (var nl in mesh.Normals)
                {
                    normals.Add(nl.X);
                    normals.Add(nl.Y);
                    normals.Add(nl.Z);
                }
            }
            
            if (mesh.HasTextureCoords(0))
            {
                foreach (var tc in mesh.TextureCoordinateChannels[0])
                {
                    texCoords.Add(tc.X);
                    texCoords.Add(tc.Y);
                    texCoords.Add(tc.Z);
                }
            }
            
            if (mesh.HasVertices)
            {
                var vert_vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vert_vbo);
                GL.BufferData
                    (
                    BufferTarget.ArrayBuffer,
                    points.Count * sizeof(float),
                    points.ToArray(),
                    BufferUsageHint.StaticDraw
                    );
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);
            }
            if (mesh.HasNormals)
            {
                var normals_vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, normals_vbo);
                GL.BufferData
                (
                    BufferTarget.ArrayBuffer,
                    normals.Count * sizeof(float),
                    normals.ToArray(),
                    BufferUsageHint.StaticDraw
                );
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);
            }
            if (mesh.HasTextureCoords(0))
            {
                var texcoords_vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, texcoords_vbo);
                GL.BufferData
                (
                    BufferTarget.ArrayBuffer,
                    texCoords.Count * sizeof(float),
                    texCoords.ToArray(),
                    BufferUsageHint.StaticDraw
                );
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(2);
            }

            Console.WriteLine("Mesh Loaded");
            
            return true;
        }
    }
}