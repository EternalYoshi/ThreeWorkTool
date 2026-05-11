using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Geometry
{
    public class Checkerboard : IDisposable
    {
        private int vao, vbo;
        private int vertexCount;
        private int shaderProgram;

        // Configurable properties
        public int GridSize { get; set; } = 50;   // number of squares per side
        public float TileSize { get; set; } = 10.0f; // world-space size of each tile
        public Color4 ColorA { get; set; } = new Color4(0.8f, 0.8f, 0.8f, 1.0f); // light
        public Color4 ColorB { get; set; } = new Color4(0.2f, 0.2f, 0.2f, 1.0f); // dark

        //This will have its own shader independent of anything else.
        private static readonly string VertexShaderSrc = @"
        #version 330 core
        layout(location = 0) in vec3 aPosition;
        layout(location = 1) in vec4 aColor;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;

        out vec4 vColor;

        void main()
        {
            gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
            vColor = aColor;
        }";

        private static readonly string FragmentShaderSrc = @"
        #version 330 core
        in vec4 vColor;
        out vec4 FragColor;

        void main()
        {
            FragColor = vColor;
        }";

        public void Load()
        {
            shaderProgram = CreateShaderProgram(VertexShaderSrc, FragmentShaderSrc);
            BuildMesh();
        }

        private void BuildMesh()
        {
            var vertices = new List<float>();
            float halfSize = (GridSize * TileSize) / 2.0f;

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    float x = -halfSize + col * TileSize;
                    float z = -halfSize + row * TileSize;

                    Color4 color = (row + col) % 2 == 0 ? ColorA : ColorB;

                    // Each tile = 2 triangles (6 vertices)
                    // Triangle 1
                    AddVertex(vertices, x, 0, z, color);
                    AddVertex(vertices, x + TileSize, 0, z, color);
                    AddVertex(vertices, x + TileSize, 0, z + TileSize, color);
                    // Triangle 2
                    AddVertex(vertices, x, 0, z, color);
                    AddVertex(vertices, x + TileSize, 0, z + TileSize, color);
                    AddVertex(vertices, x, 0, z + TileSize, color);
                }
            }

            vertexCount = vertices.Count / 7; // 3 pos + 4 color floats per vertex
            float[] data = vertices.ToArray();

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

            // Position: location 0, 3 floats
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Color: location 1, 4 floats
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        private void AddVertex(List<float> list, float x, float y, float z, Color4 color)
        {
            list.Add(x);
            list.Add(y);
            list.Add(z);
            list.Add(color.R);
            list.Add(color.G);
            list.Add(color.B);
            list.Add(color.A);
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            GL.UseProgram(shaderProgram);

            Matrix4 model = Matrix4.Identity; // centered at 0,0,0
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uProjection"), false, ref projection);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
            GL.BindVertexArray(0);
        }

        private int CreateShaderProgram(string vertSrc, string fragSrc)
        {
            int vert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert, vertSrc);
            GL.CompileShader(vert);

            int frag = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag, fragSrc);
            GL.CompileShader(frag);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vert);
            GL.AttachShader(program, frag);
            GL.LinkProgram(program);

            GL.DeleteShader(vert);
            GL.DeleteShader(frag);

            return program;
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);
            GL.DeleteProgram(shaderProgram);
        }

    }
}
