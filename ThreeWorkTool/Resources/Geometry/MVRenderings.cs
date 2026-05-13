using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ThreeWorkTool.Resources.Geometry;
using ThreeWorkTool.Resources.Wrappers.ModelNodes;

namespace ThreeWorkTool.Resources.Geometry
{
    //Based on the Checkerboard class which means it has its own shader.
    public class Sphere : IDisposable
    {
        private int vao, vbo, ebo;
        private int shaderProgram;
        private int indexCount;
        private bool Disposed = false;

        public float Radius { get; set; } = 1f;
        public int Stacks { get; set; } = 16;
        public int Slices { get; set; } = 16;
        public Color4 Color { get; set; } = new Color4(0.4f, 0.9f, 0.0f, 1.0f);
        public bool Wireframe { get; set; } = false;

        private static readonly string VertSrc = @"
    #version 330 core
    layout(location = 0) in vec3 aPosition;

    uniform mat4 uModel;
    uniform mat4 uView;
    uniform mat4 uProjection;

    void main()
    {
        gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    }";

        private static readonly string FragSrc = @"
    #version 330 core
    uniform vec4 uColor;
    out vec4 FragColor;

    void main()
    {
        FragColor = uColor;
    }";

        public void Load()
        {
            shaderProgram = BuildShader(VertSrc, FragSrc);
            BuildMesh();
        }

        private void BuildMesh()
        {
            var verts = new List<float>();
            var indices = new List<int>();

            // Generate vertices (UV sphere)
            for (int stack = 0; stack <= Stacks; stack++)
            {
                float phi = MathHelper.Pi * stack / Stacks; // 0 to PI
                for (int slice = 0; slice <= Slices; slice++)
                {
                    float theta = 2f * MathHelper.Pi * slice / Slices; // 0 to 2PI

                    float x = Radius * (float)(Math.Sin(phi) * Math.Cos(theta));
                    float y = Radius * (float)Math.Cos(phi);
                    float z = Radius * (float)(Math.Sin(phi) * Math.Sin(theta));

                    verts.Add(x);
                    verts.Add(y);
                    verts.Add(z);
                }
            }

            // Generate indices
            for (int stack = 0; stack < Stacks; stack++)
            {
                for (int slice = 0; slice < Slices; slice++)
                {
                    int a = stack * (Slices + 1) + slice;
                    int b = (stack + 1) * (Slices + 1) + slice;

                    // Two triangles per quad
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(a + 1);

                    indices.Add(b);
                    indices.Add(b + 1);
                    indices.Add(a + 1);
                }
            }

            indexCount = indices.Count;

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                verts.Count * sizeof(float),
                verts.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                indices.Count * sizeof(int),
                indices.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        public void Render(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            GL.UseProgram(shaderProgram);

            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "uProjection"), false, ref projection);
            GL.Uniform4(GL.GetUniformLocation(shaderProgram, "uColor"), Color);

            if (Wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            if (Wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill); // restore
        }

        private int BuildShader(string vertSrc, string fragSrc)
        {
            int vert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert, vertSrc);
            GL.CompileShader(vert);

            int frag = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag, fragSrc);
            GL.CompileShader(frag);

            int prog = GL.CreateProgram();
            GL.AttachShader(prog, vert);
            GL.AttachShader(prog, frag);
            GL.LinkProgram(prog);

            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
            return prog;
        }

        public void Dispose()
        {

            //Meant to avoid double disposing.
            if (Disposed)
            {
                return;
            }
            Disposed = true;

            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteVertexArray(vao);
            GL.DeleteProgram(shaderProgram);
            GC.SuppressFinalize(this);
        }
    }

    public class MVRenderings
    {

        public void ShapesForBones(List<ModelBoneEntry> Joints, TheRenderer theRenderer, FrmModelViewer modelViewer)
        {

            for(int j = 0; j < Joints.Count; j++)
            {
                //Sphere.Render(Joints[j].);


            }



        }

    }
}
